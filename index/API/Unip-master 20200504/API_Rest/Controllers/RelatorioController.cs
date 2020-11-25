using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using API_Rest.Classes;
using API_Rest.Models;
using System.Data;
using ClosedXML.Excel;
using System.Text.RegularExpressions;
using System.IO;

namespace API_Rest.Controllers
{
    public class RelatorioController : Controller
    {
        [HttpPost]
        public JsonResult relatorio(Relatorio relatorio)
        {
            Retorno Retorno = processaRelatorio(relatorio);
            return API_Rest.Json.Serialize(Retorno);
        }

        public Retorno processaRelatorio(Relatorio relatorio)
        {
            DB Conn = new DB();

            Retorno Retorno = new Retorno();
            Retorno.donwload = new List<Link>();
            try
            {
                if (Conn.Open()) 
                {
                    var excel = new XLWorkbook();

                    string nom_file = "UNIP_AudioVisual";

                    var Diretorio = HttpRuntime.AppDomainAppPath.ToString().Replace("\\", "/");

                    DateTime dataAtual = DateTime.Now;
                    var dataAtualSplit = dataAtual.ToString().Split(' ')[0];
                    var horaAtualSplit = dataAtual.ToString().Split(' ')[1];

                    Random random = new Random();
                    int num = random.Next(10000, 999999999);

                    nom_file += "_" + dataAtualSplit + "_" + horaAtualSplit + "_" + num + ".xlsx";
                    nom_file = Regex.Replace(nom_file, "/", "", RegexOptions.IgnoreCase);
                    nom_file = Regex.Replace(nom_file, ":", "", RegexOptions.IgnoreCase);

                    string path = Diretorio + "arquivos";

                    if (!Directory.Exists(path))
                    {
                        DirectoryInfo dir = Directory.CreateDirectory(path);

                        path = Diretorio + "xlsx";
                        DirectoryInfo dir1 = Directory.CreateDirectory(path);
                    }

                    var LocalSave = Diretorio + "arquivos/xlsx/" + nom_file;
                    var LocalLink = @"/backend/arquivos/xlsx/" + nom_file;

                    var query = " SELECT colab.cod_colaborador, " +
                                "        colab.nome_colaborador, " +
                                "        colab.funcional, " +
                                "        cast(agend.data_reserva as varchar), " +
                                "        cast(agend.hora_reserva as varchar), " +
                                "        cast(agend.agendado_para as varchar), " +
                                "        cast(agend.horario_de as varchar), " +
                                "        cast(agend.horario_ate as varchar), " +
                                "        cast(agend.data_entrega as varchar), " +
                                "        cast(agend.hora_entrega as varchar), " +
                                "        equip.num_controle, " +
                                "        equip.marca, " +
                                "        equip.tipo, " +
                                "        equip.modelo " +
                                " FROM app_colaboradores AS colab, " +
                                "      app_agendamento AS agend, " +
                                "      app_equipamentos AS equip, " +
                                "      app_equipamentos_agendados AS ea " +
                                " WHERE agend.cod_colaborador = colab.cod_colaborador " +
                                "   AND ea.cod_equipamento = equip.cod_equipamento " +
                                "   AND agend.cod_agendamento = ea.cod_agendamento ";

                    if (relatorio.cod_colaborador != "" && relatorio.cod_colaborador != null)
                    {
                        query += " AND agend.cod_colaborador = " + relatorio.cod_colaborador;
                    }

                    if ((relatorio.mes_de != "" && relatorio.mes_de != null) && (relatorio.mes_ate == "" || relatorio.mes_ate == null))
                    {
                        query += " AND Extract('Month' from agend.agendado_para) = " + relatorio.mes_de;
                    }

                    if ((relatorio.mes_de != "" && relatorio.mes_de != null) && (relatorio.mes_ate != "" && relatorio.mes_ate != null))
                    {
                        query += " AND Extract('Month' from agend.agendado_para) >= " + relatorio.mes_de;
                        query += " AND Extract('Month' from agend.agendado_para) <= " + relatorio.mes_ate;
                    }

                    query += "ORDER BY agend.data_reserva";

                    DataTable dtRelatorio = Conn.getDataTable(query);

                    var nome = "UNIP_AudioVisual_" + dataAtualSplit;
                    nome = Regex.Replace(nome, "/", "", RegexOptions.IgnoreCase);

                    var worksheet = excel.Worksheets.Add(nome);
                    worksheet.Style.Font.FontName = "Arial";

                    int row = 1;

                    worksheet.Style.Font.FontSize = 9;

                    worksheet.Cell(row, 1).Value = "Relatório Mensal de Agendamentos";
                    worksheet.Cell(row, 1).Style.Font.FontSize = 16;
                    worksheet.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell(row, 1).Style.Font.Bold = true;
                    worksheet.Range(row, 1, row, 14).Merge();

                    worksheet.Column(1).Width = 17.56;
                    worksheet.Column(2).Width = 18.89;
                    worksheet.Column(3).Width = 16.11;
                    worksheet.Column(4).Width = 17.89;
                    worksheet.Column(5).Width = 17.89;
                    worksheet.Column(6).Width = 17.89;
                    worksheet.Column(7).Width = 17.89;
                    worksheet.Column(8).Width = 17.89;
                    worksheet.Column(9).Width = 17.89;
                    worksheet.Column(10).Width = 17.89;
                    worksheet.Column(11).Width = 17.89;
                    worksheet.Column(12).Width = 17.89;
                    worksheet.Column(13).Width = 17.89;
                    worksheet.Column(14).Width = 17.89;

                    row++; //PROXIMA LINHA
                    worksheet.Cell(row, 1).Style.Font.Bold = true;
                    worksheet.Cell(row, 1).Value = "Dt Processamento: ";
                    worksheet.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    worksheet.Cell(row, 2).Value = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    worksheet.Cell(row, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    worksheet.Cell(row, 5).Style.Font.Bold = true;
                    worksheet.Cell(row, 5).Value = "Campus: ";
                    worksheet.Cell(row, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    worksheet.Cell(row, 6).Value = "Unip Swift";
                    worksheet.Cell(row, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    worksheet.Cell(row, 9).Style.Font.Bold = true;
                    worksheet.Cell(row, 9).Value = "Processado Por: ";
                    worksheet.Cell(row, 9).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    worksheet.Cell(row, 10).Value = relatorio.processado_por;
                    worksheet.Cell(row, 10).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    row++; // proxima linha
                    row++;       //cabeçalho
                    worksheet.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#b7dee8");
                    worksheet.Cell(row, 1).Style.Font.Bold = true;
                    worksheet.Cell(row, 1).Value = "COD COLABORADOR";

                    worksheet.Cell(row, 2).Style.Fill.BackgroundColor = XLColor.FromHtml("#b7dee8");
                    worksheet.Cell(row, 2).Style.Font.Bold = true;
                    worksheet.Cell(row, 2).Value = "NOME COLABORADOR";

                    worksheet.Cell(row, 3).Style.Fill.BackgroundColor = XLColor.FromHtml("#b7dee8");
                    worksheet.Cell(row, 3).Style.Font.Bold = true;
                    worksheet.Cell(row, 3).Value = "FUNCIONAL";

                    worksheet.Cell(row, 4).Style.Fill.BackgroundColor = XLColor.FromHtml("#b7dee8");
                    worksheet.Cell(row, 4).Style.Font.Bold = true;
                    worksheet.Cell(row, 4).Value = "DATA RESERVA";

                    worksheet.Cell(row, 5).Style.Fill.BackgroundColor = XLColor.FromHtml("#b7dee8");
                    worksheet.Cell(row, 5).Style.Font.Bold = true;
                    worksheet.Cell(row, 5).Value = "HORA RESERVA";

                    worksheet.Cell(row, 6).Style.Fill.BackgroundColor = XLColor.FromHtml("#b7dee8");
                    worksheet.Cell(row, 6).Style.Font.Bold = true;
                    worksheet.Cell(row, 6).Value = "AGENDADO PARA";

                    worksheet.Cell(row, 7).Style.Fill.BackgroundColor = XLColor.FromHtml("#b7dee8");
                    worksheet.Cell(row, 7).Style.Font.Bold = true;
                    worksheet.Cell(row, 7).Value = "HORARIO DE";

                    worksheet.Cell(row, 8).Style.Fill.BackgroundColor = XLColor.FromHtml("#b7dee8");
                    worksheet.Cell(row, 8).Style.Font.Bold = true;
                    worksheet.Cell(row, 8).Value = "HORARIO ATE";

                    worksheet.Cell(row, 9).Style.Fill.BackgroundColor = XLColor.FromHtml("#b7dee8");
                    worksheet.Cell(row, 9).Style.Font.Bold = true;
                    worksheet.Cell(row, 9).Value = "DATA ENTREGA";

                    worksheet.Cell(row, 10).Style.Fill.BackgroundColor = XLColor.FromHtml("#b7dee8");
                    worksheet.Cell(row, 10).Style.Font.Bold = true;
                    worksheet.Cell(row, 10).Value = "HORA ENTREGA";

                    worksheet.Cell(row, 11).Style.Fill.BackgroundColor = XLColor.FromHtml("#b7dee8");
                    worksheet.Cell(row, 11).Style.Font.Bold = true;
                    worksheet.Cell(row, 11).Value = "NUM CONTROLE";

                    worksheet.Cell(row, 12).Style.Fill.BackgroundColor = XLColor.FromHtml("#b7dee8");
                    worksheet.Cell(row, 12).Style.Font.Bold = true;
                    worksheet.Cell(row, 12).Value = "MARCA";

                    worksheet.Cell(row, 13).Style.Fill.BackgroundColor = XLColor.FromHtml("#b7dee8");
                    worksheet.Cell(row, 13).Style.Font.Bold = true;
                    worksheet.Cell(row, 13).Value = "TIPO";

                    worksheet.Cell(row, 14).Style.Fill.BackgroundColor = XLColor.FromHtml("#b7dee8");
                    worksheet.Cell(row, 14).Style.Font.Bold = true;
                    worksheet.Cell(row, 14).Value = "MODELO";

                    foreach (DataRow rowRelat in dtRelatorio.Rows)
                    {
                        row++;
                        worksheet.Cell(row, 1).Value = rowRelat["cod_colaborador"].ToString();
                        worksheet.Cell(row, 2).Value = rowRelat["nome_colaborador"].ToString();
                        worksheet.Cell(row, 3).Value = rowRelat["funcional"].ToString();
                        worksheet.Cell(row, 4).Value = rowRelat["data_reserva"].ToString();
                        worksheet.Cell(row, 5).Value = rowRelat["hora_reserva"].ToString();
                        worksheet.Cell(row, 6).Value = rowRelat["agendado_para"].ToString();
                        worksheet.Cell(row, 7).Value = rowRelat["horario_de"].ToString();
                        worksheet.Cell(row, 8).Value = rowRelat["horario_ate"].ToString();
                        worksheet.Cell(row, 9).Value = rowRelat["data_entrega"].ToString();
                        worksheet.Cell(row, 10).Value = rowRelat["hora_entrega"].ToString();
                        worksheet.Cell(row, 11).Value = rowRelat["num_controle"].ToString();
                        worksheet.Cell(row, 12).Value = rowRelat["marca"].ToString();
                        worksheet.Cell(row, 13).Value = rowRelat["tipo"].ToString();
                        worksheet.Cell(row, 14).Value = rowRelat["modelo"].ToString();

                    }

                    excel.SaveAs(LocalSave);

                    Link l = new Link();
                    l.link = LocalLink;
                    Retorno.donwload.Add(l);
                }
                else
                {
                    throw new Exception("A conexão com o banco de dados foi encerrada de forma inesperada.");
                }

                if (Retorno.donwload[0].link != "")
                {
                    Retorno.status = true;
                    Retorno.msg = "Processamento concluido com sucesso!";
                }
                else
                {
                    throw new Exception("Falha ao gerar relatorio.");
                }
                
            }
            catch (Exception e)
            {

                Retorno.status = false;
                Retorno.erro = e.Message;
            }

            Conn.Close();
            return Retorno;
        }

        public JsonResult select(Relatorio relatorio)
        {
            Retorno Retorno = processaSelect(relatorio);
            return API_Rest.Json.Serialize(Retorno);
        }

        public Retorno processaSelect(Relatorio relatorio)
        {   
            DB Conn = new DB();
            Retorno Retorno = new Retorno();
            Retorno.ListaRelatorio = new List<Relatorio>();
            try
            {
                if (Conn.Open())
                {
                    var query = " SELECT colab.cod_colaborador, " +
                                "        colab.nome_colaborador, " +
                                "        colab.funcional, " +
                                "        agend.data_reserva, " +
                                "        agend.hora_reserva, " +
                                "        agend.agendado_para, " +
                                "        agend.horario_de, " +
                                "        agend.horario_ate, " +
                                "        agend.data_entrega, " +
                                "        agend.hora_entrega, " +
                                "        equip.num_controle, " +
                                "        equip.marca, " +
                                "        equip.tipo, " +
                                "        equip.modelo " +
                                " FROM app_colaboradores AS colab, " +
                                "      app_agendamento AS agend, " +
                                "      app_equipamentos AS equip, " +
                                "      app_equipamentos_agendados AS ea " +
                                " WHERE agend.cod_colaborador = colab.cod_colaborador " +
                                "   AND ea.cod_equipamento = equip.cod_equipamento " +
                                "   AND agend.cod_agendamento = ea.cod_agendamento ";

                    if (relatorio.cod_colaborador != "" && relatorio.cod_colaborador != null)
                    {
                        query += " AND agend.cod_colaborador = " + relatorio.cod_colaborador;
                    }

                    if ((relatorio.mes_de != "" && relatorio.mes_de != null) && (relatorio.mes_ate == "" || relatorio.mes_ate == null))
                    {
                        query += " AND Extract('Month' from agend.agendado_para) = " + relatorio.mes_de;
                    }

                    if ((relatorio.mes_de != "" && relatorio.mes_de != null) && (relatorio.mes_ate != "" && relatorio.mes_ate != null))
                    {
                        query += " AND Extract('Month' from agend.agendado_para) >= " + relatorio.mes_de;
                        query += " AND Extract('Month' from agend.agendado_para) <= " + relatorio.mes_ate;
                    }

                    query += " ORDER BY agendado_para DESC";

                    DataTable dtRelatorio = Conn.getDataTable(query);
                    foreach (DataRow row in dtRelatorio.Rows)
                    {
                        Relatorio relat = new Relatorio();

                        relat.cod_colaborador   = row["cod_colaborador"].ToString();
                        relat.nome_colaborador  = row["nome_colaborador"].ToString();
                        relat.funcional         = row["funcional"].ToString();
                        relat.data_reserva      = row["data_reserva"].ToString();
                        relat.hora_reserva      = row["hora_reserva"].ToString();
                        relat.agendado_para     = row["agendado_para"].ToString();
                        relat.horario_de        = row["horario_de"].ToString();
                        relat.horario_ate       = row["horario_ate"].ToString();
                        relat.data_entrega      = row["data_entrega"].ToString();
                        relat.hora_entrega      = row["hora_entrega"].ToString();
                        relat.num_controle      = row["num_controle"].ToString();
                        relat.marca             = row["marca"].ToString();
                        relat.tipo              = row["tipo"].ToString();
                        relat.modelo            = row["modelo"].ToString();
                        //relat.cod_equipamento   = row["cod_equipamento"].ToString();
                        //relat.mes_de            = row["mes_de"].ToString();
                        //relat.mes_ate           = row["mes_ate"].ToString();

                        Retorno.ListaRelatorio.Add(relat);
                    }

                    if (Retorno.ListaRelatorio.Count() == 0)
                    {
                        throw new Exception("Nenhum registro a ser exibido.");
                    }
                }
                else
                {
                    throw new Exception("A conexão com o banco de dados foi encerrada de forma inesperada.");
                }
                Retorno.status = true;
                Retorno.msg = "Processamento concluido com sucesso!";
            }
            catch (Exception e)
            {

                Retorno.status = false;
                Retorno.erro = e.Message;
            }
            Conn.Close();
            return Retorno;
        }
    }
}