using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using API_Rest.Classes;
using API_Rest.Models;
using System.Data;

namespace API_Rest.Controllers
{
    public class AgendamentoController : Controller
    {
        [HttpPost]
        public JsonResult insert(Agendamento agendamento)
        {
            Retorno Retorno = processaInsert(agendamento);
            return API_Rest.Json.Serialize(Retorno);
        }

        public Retorno processaInsert(Agendamento agendamento)
        {
            DB Conn = new DB();
            Retorno Retorno = new Retorno();

            List<Email> ListaEmail = new List<Email>();
            try
            {
                if (Conn.Open())
                {
                    if (agendamento.Equipamento.Count == 0)
                    {
                       throw new Exception("Por favor selecionar o Equipamento que deseja reservar.");
                    }

                    Conn.Begin();
                    var query = "SELECT * FROM app_agendamento WHERE cod_colaborador = '" + agendamento.cod_colaborador + "' " +
                                "AND cod_status_agendamento != 4 AND cod_status_agendamento != 6";
                    Conn.Query(query);

                    if (Conn.getRows() > 0)
                    {
                        throw new Exception("Colaborador já possui solicitação em andamento.");
                    }

                    query = "INSERT INTO app_agendamento(cod_colaborador, " +
                                                        "cod_status_agendamento, " +
                                                        "data_reserva, " +
                                                        "hora_reserva, " +
                                                        "cod_agendado_por, " +
                                                        "agendado_para, " +
                                                        "horario_de, " +
                                                        "horario_ate) " +
                                             "VALUES ('" + agendamento.cod_colaborador + "'," + // Responsvael pelo agendamento(Professor ou Administrador)
                                                     "'1'," +
                                                     "current_date, " +
                                                     "current_time(0) , " +
                                                     "'" + agendamento.cod_agendado_por + "'," + // PROFESSOR OU ADMINISTRADOR
                                                     "'" + agendamento.agendado_para + "', " +   // Data do agendamento
                                                     "'" + agendamento.horario_de + "', " +
                                                     "'" + agendamento.horario_ate + "' )";
                    if (!Conn.Execute(query))   
                    {
                        throw new Exception("Falha ao inserir registro na tabela app_agendamento. " + Conn.ErrorMsg);
                    }

                    if (agendamento.Equipamento.Count > 0)
                    {
                        var cod_agendamento = Conn.getID("app_agendamento", "cod_agendamento", "cod_agendamento");
                        for (int e = 0; e < agendamento.Equipamento.Count; e++)
                        {
                            query = "INSERT INTO app_equipamentos_agendados(cod_agendamento, cod_equipamento) " +
                                    "VALUES ('"+ cod_agendamento + "', " +
                                             "'"+ agendamento.Equipamento[e].cod_equipamento + "')";
                            if (!Conn.Execute(query))
                            {
                                throw new Exception("Falha ao inserir registro na tabela app_equipamentos_agendados. " + Conn.ErrorMsg);
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("É obrigatório a seleção de equipamento. ");
                    }
                }
                else
                {
                    throw new Exception("A conexão com o banco de dados foi encerrada de forma inesperada.");
                }

                Conn.Commit();
                Retorno.status = true;
                Retorno.msg = "Agendamento realizado. Enviaremos um e-mail com a descrição e confirmação do agendamento";

                //SELECT PARA CAPTURAR OS DADOS DA SOLICITAÇÃO EFETUADA PELO COLABORADOR
                string idAgendamento = Conn.getID("app_agendamento", "cod_agendamento", "cod_agendamento");
                var query_envioemail = "SELECT colab.nome_colaborador, colab.email, equip.marca AS equipamento_marca, equip.tipo AS equipamento_tipo, equip.modelo AS equipamento_modelo, status.tipo_status_agendamento " +
                        "FROM app_agendamento agend, app_colaboradores colab, app_equipamentos equip, app_status_agendamento status " +
                        "WHERE agend.cod_colaborador = colab.cod_colaborador " +
                        "AND agend.cod_equipamento = equip.cod_equipamento " +
                        "AND agend.cod_status_agendamento = status.cod_status_agendamento " +
                        "AND agend.cod_agendamento = '" + idAgendamento + "' ";

                Conn.Query(query_envioemail);
                
                var body = "";
                if (Conn.getRows() > 0)
                {
                    DateTime dateTime = DateTime.UtcNow.Date;

                    // CORPO DO EMAIL CONTENDO DADOS DA SOLICITACAO
                    body = "Solicitante : " + Conn.getValueByName("nome_colaborador") + " <br/> " +
                            "Data da Solicitação: " + dateTime.ToString("dd/MM/yyyy") + " <br/> " +
                            "<hr>" +
                            "Equipamento Solicitado: " + " <br/> " +
                            "Marca: " + Conn.getValueByName("equipamento_marca") + " <br/> " +
                            "Tipo: " + Conn.getValueByName("equipamento_tipo") + " <br/> " +
                            "Modelo: " + Conn.getValueByName("equipamento_modelo") + " <br/> " + 
                            "Situação Agendamento : " + Conn.getValueByName("tipo_status_agendamento") + " <br/> ";

                    GetEmail getEmail = new GetEmail();

                    // FUNCAO BUSCA EMAIL COLABORADORES
                    ListaEmail = getEmail.getEmailColaborador(ListaEmail, Conn, agendamento.cod_colaborador);

                    // FUNCAO BUSCA EMAIL ADMINISTRADORES
                    ListaEmail = getEmail.getEmailAdmin(ListaEmail, Conn);

                    //EFETUA O ENVIO DE EMAIL
                    EnviaEmail enviar = new EnviaEmail();
                    enviar.sendEmail(body, ListaEmail, "Nova Solicitação de Reserva");
                }
            }
            catch (Exception e)
            {
                Conn.Rollback();
                Retorno.status = false;
                Retorno.erro = e.Message;
            }

            Conn.Close();
            return Retorno;
        }

        [HttpPost]
        public JsonResult update(Agendamento agendamento)
        {
            Retorno Retorno = processaUpdate(agendamento);
            return API_Rest.Json.Serialize(Retorno);
        }

        public Retorno processaUpdate(Agendamento agendamento)
        {
            DB Conn = new DB();
            Retorno Retorno = new Retorno();
            
            List<Email> ListaEmail = new List<Email>();
            try
            {
                if (Conn.Open())
                {
                    Conn.Begin();
                    //var query = "SELECT * FROM app_agendamento WHERE cod_colaborador = '" + agendamento.cod_colaborador + "' AND cod_status_agendamento != 4 AND cod_status_agendamento != 6";
                    //Conn.Query(query);

                    //if (Conn.getRows() > 0)
                    //{
                    //    throw new Exception("Tipo de status já cadastrado.");
                    //}
                    // data_entrega = NOW()::timestamp, hora_entrega = NOW()::timestamp,
                    var query = "";
                    var status_agendamento = "";
                    if (agendamento.cod_status_agendamento != 4)
                    {
                        query = "SELECT cod_status_agendamento FROM app_agendamento WHERE cod_agendamento = '" + agendamento.cod_agendamento + "'";
                        Conn.Query(query);

                        status_agendamento = Conn.getValueByName("cod_status_agendamento");

                        switch (status_agendamento)
                        {
                            case "1": // AGENDADO
                                agendamento.cod_status_agendamento = 2; // AGUARDANDO RETIRADA
                                break;

                            case "2":
                                agendamento.cod_status_agendamento = 3; // AGUARDANDO ENTREGA
                                break;

                            case "3":
                                agendamento.cod_status_agendamento = 6; // ENTREGA CONFIRMADA
                                break;

                            case "5": // SOLICITACAO EM ATRASO
                                agendamento.cod_status_agendamento = 4; // SOLICITACAO CANCELADA
                                break;
                        }
                    }
                    

                    query = "UPDATE app_agendamento SET cod_status_agendamento  = '" + agendamento.cod_status_agendamento + "', " +
                                                       "cod_resp_alteracao      = '" + agendamento.cod_resp_alteracao + "' ";
                    if (status_agendamento == "3")
                    {
                        query += ", data_entrega = current_date" +
                                 ", hora_entrega = current_time(0) ";
                    }
                    query += "WHERE cod_agendamento       = '" + agendamento.cod_agendamento + "' ";

                    if (!Conn.Execute(query))
                    {
                        throw new Exception("Falha ao atualizar registro na tabela app_agendamento. " + Conn.ErrorMsg);
                    }
                }
                else
                {
                    throw new Exception("A conexão com o banco de dados foi encerrada de forma inesperada.");
                }


                Conn.Commit();
                Retorno.status = true;
                Retorno.msg = "Processamento concluido com sucesso";

                //SELECT PARA CAPTURAR OS DADOS DA SOLICITAÇÃO EFETUADA PELO COLABORADOR
                var query_envioemail = "SELECT colab.nome_colaborador, " +
                                               "colab.email, " +
                                               "equip.marca AS equipamento_marca, " +
                                               "equip.tipo AS equipamento_tipo, " +
                                               "equip.modelo AS equipamento_modelo, " +
                                               "status.tipo_status_agendamento, " +
                                               "agend.data_reserva, " +
                                               "agend.data_entrega " +
                                        "FROM app_agendamento agend, app_colaboradores colab, app_equipamentos equip, app_status_agendamento status " +
                                        "WHERE agend.cod_colaborador = colab.cod_colaborador " +
                                        "AND agend.cod_equipamento = equip.cod_equipamento " +
                                        "AND agend.cod_status_agendamento = status.cod_status_agendamento " +
                                        "AND agend.cod_agendamento = '" + agendamento.cod_agendamento + "' ";

                Conn.Query(query_envioemail);

                var body = "";
                if (Conn.getRows() > 0)
                {
                    DateTime dateTime = DateTime.UtcNow.Date;

                    // CORPO DO EMAIL CONTENDO DADOS DA SOLICITACAO
                    body = "Colaborador " + Conn.getValueByName("nome_colaborador") + " ocorreu alteração em sua solicitação na data " + dateTime.ToString("dd/MM/yyyy") + " " + " <br/> " +
                            "<hr>" +
                            "Equipamento Solicitado: " + " <br/> " +
                            "Marca: " + Conn.getValueByName("equipamento_marca") + " <br/> " +
                            "Tipo: " + Conn.getValueByName("equipamento_tipo") + " <br/> " +
                            "Modelo: " + Conn.getValueByName("equipamento_modelo") + " <br/> " +
                            "Situação Agendamento : " + Conn.getValueByName("tipo_status_agendamento") + " <br/> " +
                            "<hr>" +
                            "Data Reserva : " + Conn.getValueByName("data_reserva") + " <br/> " +
                            "Data Entrega : " + Conn.getValueByName("data_entrega") + " <br/> ";

                    GetEmail getEmail = new GetEmail();

                    // FUNCAO BUSCA EMAIL COLABORADORES
                    ListaEmail = getEmail.getEmailColaborador(ListaEmail, Conn, agendamento.cod_colaborador);
                    // FUNCAO BUSCA EMAIL ADMINISTRADORES
                    ListaEmail = getEmail.getEmailAdmin(ListaEmail, Conn);

                    var query_status = "SELECT * FROM app_status_agendamento WHERE cod_status_agendamento = '" + agendamento.cod_status_agendamento + "'";
                    Conn.Query(query_status);

                    var msg = Conn.getValueByName("descricao_status_agendamento");
                    //EFETUA O ENVIO DE EMAIL
                    EnviaEmail enviar = new EnviaEmail();
                    enviar.sendEmail(body, ListaEmail, msg);
                }
            }
            catch (Exception e)
            {
                Conn.Rollback();
                Retorno.status = false;
                Retorno.erro = e.Message;
            }

            Conn.Close();
            return Retorno;
        }

        [HttpPost]
        public JsonResult select()
        {
            Retorno Retorno = processaSelect();
            return API_Rest.Json.Serialize(Retorno);
        }
        public Retorno processaSelect()
        {
            Retorno Retorno = new Retorno();
            DB Conn = new DB();
            Retorno.ListaAgendamentos = new List<Agendamento>();

            try
            {
                if (Conn.Open())
                {
                    var query = "SELECT agend.*, " +
                                "colab.nome_colaborador," +
                                "(SELECT colab.nome_colaborador FROM app_colaboradores colab WHERE agend.cod_resp_alteracao = colab.cod_colaborador) as resp_alteracao, " +
                                "(SELECT colab.nome_colaborador FROM app_colaboradores colab WHERE colab.cod_colaborador = agend.cod_colaborador) as agendado_por," +
                                "status.tipo_status_agendamento, " +
                                "status.descricao_status_agendamento  " +
                                "FROM app_agendamento agend, app_colaboradores colab, app_status_agendamento status  " +
                                "WHERE agend.cod_colaborador = colab.cod_colaborador " +
                                "AND agend.cod_status_agendamento = status.cod_status_agendamento " +
                                "AND Extract('Month' From agend.data_reserva) = Extract('Month' from CURRENT_DATE)";

                    DataTable tableAgendamentos = Conn.getDataTable(query);

                    foreach (DataRow dtAgendamento in tableAgendamentos.Rows)
                    {
                        Agendamento agendamentoExistente = new Agendamento();
                        agendamentoExistente.Colaborador = new Colaboradores();
                        agendamentoExistente.Equipamento = new List<Equipamentos>();
                        agendamentoExistente.Status = new StatusAgendamento();

                        agendamentoExistente.cod_agendamento = int.Parse(dtAgendamento["cod_agendamento"].ToString());
                        agendamentoExistente.cod_colaborador = int.Parse(dtAgendamento["cod_colaborador"].ToString());
                        agendamentoExistente.cod_agendado_por = int.Parse(dtAgendamento["cod_agendado_por"].ToString());
                        agendamentoExistente.cod_resp_alteracao = dtAgendamento["cod_resp_alteracao"].ToString();
                        agendamentoExistente.cod_status_agendamento = int.Parse(dtAgendamento["cod_status_agendamento"].ToString());
                        agendamentoExistente.data_entrega = dtAgendamento["data_entrega"].ToString();
                        agendamentoExistente.hora_entrega = dtAgendamento["hora_entrega"].ToString();
                        agendamentoExistente.data_reserva = dtAgendamento["data_reserva"].ToString();
                        agendamentoExistente.agendado_para = dtAgendamento["agendado_para"].ToString();
                        agendamentoExistente.horario_de = dtAgendamento["horario_de"].ToString();
                        agendamentoExistente.horario_ate = dtAgendamento["horario_ate"].ToString();
                        agendamentoExistente.hora_reserva = dtAgendamento["hora_reserva"].ToString();
                        agendamentoExistente.agendado_por = dtAgendamento["agendado_por"].ToString();
                        agendamentoExistente.resp_alteracao = dtAgendamento["resp_alteracao"].ToString();

                        agendamentoExistente.Colaborador.cod_colaborador = int.Parse(dtAgendamento["cod_colaborador"].ToString());
                        agendamentoExistente.Colaborador.nome_colaborador = dtAgendamento["nome_colaborador"].ToString();

                        agendamentoExistente.Status.cod_status_agendamento = int.Parse(dtAgendamento["cod_status_agendamento"].ToString());
                        agendamentoExistente.Status.tipo_status_agendamento = dtAgendamento["tipo_status_agendamento"].ToString();
                        agendamentoExistente.Status.descricao_status_agendamento = dtAgendamento["descricao_status_agendamento"].ToString();

                        query = "SELECT e.* FROM app_equipamentos e, app_equipamentos_agendados ea " +
                                "WHERE e.cod_equipamento = ea.cod_equipamento " +
                                "AND ea.cod_agendamento = '"+ agendamentoExistente.cod_agendamento + "'";
                        DataTable equipamentoAgend = Conn.getDataTable(query);
                        
                        foreach (DataRow dtEquipamento in equipamentoAgend.Rows) 
                        {
                            Equipamentos equipamentos = new Equipamentos();

                            equipamentos.cod_equipamento = int.Parse(dtEquipamento["cod_equipamento"].ToString());
                            equipamentos.cod_patrimonio = int.Parse(dtEquipamento["cod_patrimonio"].ToString());
                            equipamentos.num_controle = int.Parse(dtEquipamento["num_controle"].ToString());
                            equipamentos.marca = dtEquipamento["marca"].ToString();
                            equipamentos.tipo = dtEquipamento["tipo"].ToString();
                            equipamentos.modelo = dtEquipamento["modelo"].ToString();
                            equipamentos.status_ativo = dtEquipamento["status_ativo"].ToString();

                            agendamentoExistente.Equipamento.Add(equipamentos);
                        }

                        Retorno.ListaAgendamentos.Add(agendamentoExistente);

                    }

                    if (Retorno.ListaAgendamentos.Count == 0)
                    {
                        throw new Exception("Não a registros a ser exibidos.");
                    }
                }
                else
                {
                    throw new Exception("A conexão com o banco de dados foi encerrada de forma inesperada.");
                }

                Retorno.status = true;
                Retorno.msg = "Processamento concluido com sucesso!";
            }
            catch (Exception ex)
            {
                Retorno.status = false;
                Retorno.erro = ex.Message;
            }

            Conn.Close();
            return Retorno;
        }

        [HttpPost]
        public JsonResult get(Agendamento agendamento)
        {
            Retorno Retorno = processaGet(agendamento);
            return API_Rest.Json.Serialize(Retorno);
        }
        public Retorno processaGet(Agendamento agendamento)
        {
            Retorno Retorno = new Retorno();
            DB Conn = new DB();
            Retorno.ListaAgendamentos = new List<Agendamento>();

            try
            {
                if (Conn.Open())
                {
                    
                    var query = "SELECT agend.*, " +
                                "colab.nome_colaborador," +
                                "(SELECT colab.nome_colaborador FROM app_colaboradores colab WHERE agend.cod_resp_alteracao = colab.cod_colaborador) as resp_alteracao, " +
                                "(SELECT colab.nome_colaborador FROM app_colaboradores colab WHERE colab.cod_colaborador = agend.cod_colaborador) as agendado_por," +
                                "status.tipo_status_agendamento, " +
                                "status.descricao_status_agendamento  " +
                                "FROM app_agendamento agend, app_colaboradores colab, app_status_agendamento status  " +
                                "WHERE agend.cod_colaborador = colab.cod_colaborador " +
                                "AND agend.cod_status_agendamento = status.cod_status_agendamento " +
                                "AND agend.cod_agendamento = '" + agendamento.cod_agendamento + "'";

                    DataTable tableAgendamentos = Conn.getDataTable(query);

                    foreach (DataRow dtAgendamento in tableAgendamentos.Rows)
                    {
                        Agendamento agendamentoExistente = new Agendamento();
                        agendamentoExistente.Colaborador = new Colaboradores();
                        agendamentoExistente.Equipamento = new List<Equipamentos>();
                        agendamentoExistente.Status = new StatusAgendamento();

                        agendamentoExistente.cod_agendamento = int.Parse(dtAgendamento["cod_agendamento"].ToString());
                        agendamentoExistente.cod_colaborador = int.Parse(dtAgendamento["cod_colaborador"].ToString());
                        agendamentoExistente.cod_agendado_por = int.Parse(dtAgendamento["cod_agendado_por"].ToString());
                        agendamentoExistente.cod_resp_alteracao = dtAgendamento["cod_resp_alteracao"].ToString();
                        agendamentoExistente.cod_status_agendamento = int.Parse(dtAgendamento["cod_status_agendamento"].ToString());
                        agendamentoExistente.data_entrega = dtAgendamento["data_entrega"].ToString();
                        agendamentoExistente.hora_entrega = dtAgendamento["hora_entrega"].ToString();
                        agendamentoExistente.data_reserva = dtAgendamento["data_reserva"].ToString();
                        agendamentoExistente.hora_reserva = dtAgendamento["hora_reserva"].ToString();
                        agendamentoExistente.agendado_para = dtAgendamento["agendado_para"].ToString();
                        agendamentoExistente.horario_de = dtAgendamento["horario_de"].ToString();
                        agendamentoExistente.horario_ate = dtAgendamento["horario_ate"].ToString();
                        agendamentoExistente.agendado_por = dtAgendamento["agendado_por"].ToString();
                        agendamentoExistente.resp_alteracao = dtAgendamento["resp_alteracao"].ToString();

                        agendamentoExistente.Colaborador.cod_colaborador = int.Parse(dtAgendamento["cod_colaborador"].ToString());
                        agendamentoExistente.Colaborador.nome_colaborador = dtAgendamento["nome_colaborador"].ToString();

                        agendamentoExistente.Status.cod_status_agendamento = int.Parse(dtAgendamento["cod_status_agendamento"].ToString());
                        agendamentoExistente.Status.tipo_status_agendamento = dtAgendamento["tipo_status_agendamento"].ToString();
                        agendamentoExistente.Status.descricao_status_agendamento = dtAgendamento["descricao_status_agendamento"].ToString();

                        query = "SELECT e.* FROM app_equipamentos e, app_equipamentos_agendados ea " +
                                "WHERE e.cod_equipamento = ea.cod_equipamento " +
                                "AND ea.cod_agendamento = '" + agendamentoExistente.cod_agendamento + "'";
                        DataTable equipamentoAgend = Conn.getDataTable(query);

                        foreach (DataRow dtEquipamento in equipamentoAgend.Rows)
                        {
                            Equipamentos equipamentos = new Equipamentos();

                            equipamentos.marca = dtEquipamento["marca"].ToString();
                            equipamentos.tipo = dtEquipamento["tipo"].ToString();
                            equipamentos.modelo = dtEquipamento["modelo"].ToString();
                            equipamentos.status_ativo = dtEquipamento["status_ativo"].ToString();

                            agendamentoExistente.Equipamento.Add(equipamentos);
                        }

                        Retorno.ListaAgendamentos.Add(agendamentoExistente);
                    }

                    if (Retorno.ListaAgendamentos.Count == 0)
                    {
                        throw new Exception("Não a registros a ser exibidos.");
                    }
                }
                else
                {
                    throw new Exception("A conexão com o banco de dados foi encerrada de forma inesperada.");
                }

                Retorno.status = true;
                Retorno.msg = "Processamento concluido com sucesso!";
            }
            catch (Exception ex)
            {
                Retorno.status = false;
                Retorno.erro = ex.Message;
            }

            Conn.Close();
            return Retorno;
        }

        [HttpPost]
        public JsonResult getHorario(string data)
        {
            Retorno Retorno = processaGetHorario(data);
            return API_Rest.Json.Serialize(Retorno);
        }
        public Retorno processaGetHorario(string data)
        {
            Retorno Retorno = new Retorno();
            DB Conn = new DB();
            Retorno.ListaHorarios = new List<HoraAgendamento>();

            try
            {
                if (Conn.Open())
                {
                    DateTime dateNow = DateTime.UtcNow.Date;
                    DateTime dataReserva = Convert.ToDateTime(data);

                    int compare = DateTime.Compare(dataReserva, dateNow);
                    
                    //  1  -> primeiro maior que o segundo 
                    // -1  -> o segundo é maior 
                    //  0  -> ambos sao iguais
                    if (compare == -1) 
                    {
                        throw new Exception("Não é possivel efetuar agendamentos no passado");
                    }

                    var query = "select ha.* from app_horario_agendamento ha where ha.horario_de not in (select a.horario_de from app_agendamento a where a.agendado_para = '" + data + "' and a.cod_status_agendamento <> 6)";
                    DataTable horarios = Conn.getDataTable(query);

                    foreach (DataRow row in horarios.Rows)
                    {
                        HoraAgendamento hr = new HoraAgendamento();

                        hr.cod_horario = int.Parse( row["cod_horario"].ToString() );
                        hr.horario_de = row["horario_de"].ToString();
                        hr.horario_ate = row["horario_ate"].ToString();

                        Retorno.ListaHorarios.Add(hr);
                    }

                    if (Retorno.ListaHorarios.Count == 0)
                    {
                        throw new Exception("Não a horarios disponivel para data selecionada.");
                    }
                }
                else
                {
                    throw new Exception("A conexão com o banco de dados foi encerrada de forma inesperada.");
                }

                Retorno.status = true;
                Retorno.msg = "Processamento concluido com sucesso!";
            }
            catch (Exception ex)
            {
                Retorno.status = false;
                Retorno.erro = ex.Message;
            }

            Conn.Close();
            return Retorno;
        }


        [HttpPost]
        public JsonResult verificaHorario(string hora)
        {
            Retorno Retorno = processaVerificaHorario(hora);
            return API_Rest.Json.Serialize(Retorno);
        }
        public Retorno processaVerificaHorario(string hora)
        {
            Retorno Retorno = new Retorno();
            Retorno.ListaHorarios = new List<HoraAgendamento>();

            try
            {
                //TimeSpan horaAtual = new TimeSpan(1);
                DateTime horaAtual = DateTime.Now;
                var horaAtualSplit = horaAtual.ToString().Split(' ');
                horaAtualSplit = horaAtualSplit[1].ToString().Split(':');
                  
                var horaSplit = hora.Split(':');
                TimeSpan horaSel = new TimeSpan(Int32.Parse(horaSplit[0]), Int32.Parse( horaSplit[1] ), 00);
                TimeSpan horaAtualSpan = new TimeSpan(Int32.Parse(horaAtualSplit[0]), Int32.Parse(horaAtualSplit[1]), 00);

                int horaCompare = TimeSpan.Compare(horaSel, horaAtualSpan);

                // 0 -> TEMPOS IGUAIS
                // 1 -> PRIMEIRO TEMPO É MAIOR
                // 2 -> SEGUNDO TEMPO É MAIOR
                if (horaCompare == -1) 
                {
                    throw new Exception("Não é possivel efetuar agendamentos no passado");
                }
                Retorno.status = true;
            }
            catch (Exception ex)
            {
                Retorno.status = false;
                Retorno.erro = ex.Message;
            }

            return Retorno;
        }


        public bool verifica_atraso() 
        {
            bool status = true;
            DateTime dateNow = DateTime.UtcNow.Date;

            DB Conn = new DB();

            if (Conn.Open())
            {

                // CASO STATUS AGENDAMENTO FOR AGUARDANDO ENTREGA
                // EFETUARA VERIFICACAO DE ATRASO
                var query = "select cod_agendamento, agendado_para from app_agendamento" +
                            " where cod_status_agendamento = 3";
                DataTable v = Conn.getDataTable(query);

                foreach(DataRow vrow in v.Rows)
                {
                    DateTime dataReserva = Convert.ToDateTime(vrow["agendado_para"].ToString());
                    var cod_agendamento = vrow["cod_agendamento"].ToString();
                    int compare = DateTime.Compare(dataReserva, dateNow);
                    
                    //  1  -> primeiro maior que o segundo 
                    // -1  -> o segundo é maior 
                    //  0  -> ambos sao iguais
                    if (compare == -1)
                    {
                        query = "UPDATE app_agendamento set cod_status_agendamento = 5 where cod_agendamento = '" + cod_agendamento + "'";
                        if (!(Conn.Execute(query))) 
                        {
                            status = false;
                        }
                    }
                }
            }
            else
            {
                throw new Exception("A conexão com o banco de dados foi encerrada de forma inesperada.");
            }

            return status;
        }
    }
}