
jQuery(document).ready(function(){

    jQuery("#aprovar").click(function(){
        aprovar_cancelar_agendamento(false);
    });

    jQuery("#cancelar").click(function(){
        aprovar_cancelar_agendamento(true);
    });

});

function listaAgendamentos(carregar = true)
{
    consultaWS("agendamento", "select", null, function(Retorno)
    {
        if(Retorno.status)
        {
            console.log(Retorno);

            var agendado_para               = "";

            var count_agendado              = 0;
            var count_aguardando_retirada   = 0;
            var count_aguardando_entrega    = 0;
            var count_entrega_confirmada    = 0;
            var count_solicitacao_cancelada = 0;
            var count_solicitacao_atraso    = 0;

            var divStatusAgend = "";

            var idhtml = "";
            for (let a = 0; a < Retorno.ListaAgendamentos.length; a++) 
            {
                var gethtml = "";
                var agendamento = Retorno.ListaAgendamentos[a];

                var cod_status_agendamento = agendamento.cod_status_agendamento;

                switch(cod_status_agendamento)
                {
                    case 1:
                        divStatusAgend = jQuery("#agendado");

                        count_agendado += 1;
                    break;

                    case 2:
                        divStatusAgend = jQuery("#aguardando_retirada");

                        count_aguardando_retirada += 1;
                    break;

                    case 3:
                        divStatusAgend = jQuery("#aguardando_entrega");

                        count_aguardando_entrega += 1;
                    break;

                    case 4:
                        divStatusAgend = jQuery("#solicitacao_cancelada");

                        count_solicitacao_cancelada += 1;
                    break;

                    case 5:
                        divStatusAgend = jQuery("#solicitacao_em_atraso");

                        count_solicitacao_atraso += 1;
                    break;

                    case 6:
                        divStatusAgend = jQuery("#entrega_confirmada");

                        count_entrega_confirmada += 1;
                    break;
                }

                agendado_para   = formate_date(agendamento.agendado_para.split(" ")[0]); 

                gethtml = getHtmlAgendamento();
                gethtml = gethtml.replace("[COLABORADOR]", agendamento.Colaborador.nome_colaborador);
                gethtml = gethtml.replace("[DATA]", agendado_para);
                gethtml = gethtml.replace("[HORA]", agendamento.horario_de);
                gethtml = gethtml.replace("[ID]", agendamento.cod_agendamento);

                divStatusAgend.append(gethtml);

                jQuery("#count_agendado").text(count_agendado);
                jQuery("#count_aguardando_retirada").text(count_aguardando_retirada);
                jQuery("#count_aguardando_entrega").text(count_aguardando_entrega);
                jQuery("#count_entrega_confirmada").text(count_entrega_confirmada);
                jQuery("#count_solicitacao_cancelada").text(count_solicitacao_cancelada);
                jQuery("#count_solicitacao_atraso").text(count_solicitacao_atraso);

                jQuery("#buttonAgend_"+agendamento.cod_agendamento).click(function(evt){
                    getAgendamento(evt.target.id);
                });
            }
        }
        else
        {
            Notificar(Retorno.erro);
        }
    },carregar);
}

function getAgendamento(evt)
{
    var idButton = evt;
    var cod_agendamento = idButton.replace("buttonAgend_", "");

    var dados = {
        cod_agendamento : cod_agendamento
    }

    consultaWS("agendamento", "get", dados, function(Retorno)
    {
        if(Retorno.status)
        {
            console.log(Retorno);

            var cod_agendamento = Retorno.ListaAgendamentos[0].cod_agendamento;
            var agendado_por    = Retorno.ListaAgendamentos[0].agendado_por;
            var agendado_em     = formate_date(Retorno.ListaAgendamentos[0].data_reserva.split(" ")[0]);
            var agendado_para   = formate_date(Retorno.ListaAgendamentos[0].agendado_para.split(" ")[0]);
            var horario_de_ate  = Retorno.ListaAgendamentos[0].horario_de + " - " + Retorno.ListaAgendamentos[0].horario_ate;

            jQuery("#cod_agendamento").val(cod_agendamento).prop("disabled", true);
            jQuery("#solicitante").val(agendado_por).prop("disabled", true);
            jQuery("#agendado_em").val(agendado_em).prop("disabled", true);
            jQuery("#agendado_para").val(agendado_para).prop("disabled", true);
            jQuery("#horario_de_ate").val(horario_de_ate).prop("disabled", true);

            listaEquipamentosAgendados(Retorno);

            var cod_status_agendamento = Retorno.ListaAgendamentos[0].Status.cod_status_agendamento;
            if(cod_status_agendamento == 6 || cod_status_agendamento == 4 )
            {
                jQuery("#aprovar").prop("disabled", true);
                jQuery("#cancelar").prop("disabled", true);
            }
            else
            {
                jQuery("#aprovar").prop("disabled", false);
                jQuery("#cancelar").prop("disabled", false)
            }
        }
    },false);
}

function getHtmlAgendamento()
{
    var gethtml =   '<div class="col-6 col-lg-3">' +
                    '    <div class="card">' +
                    '        <div class="card-body">' +
                    '            <div class="clearfix">' +
                    '                <i class="fa fa-spinner bg-success p-3 font-2xl mr-3 float-left text-light" style="border-radius: 50px;"></i>' +
                    '                <div class="h5 text-secondary mb-0 mt-1">[COLABORADOR]</div>' +
                    '                <div class="text-muted text-uppercase font-weight-bold font-xs small" style="font-size: 8px;">Data: [DATA]</div>' +
                    '                <div class="text-muted text-uppercase font-weight-bold font-xs small" style="font-size: 8px;">Hora: [HORA]</div>' +
                    '            </div>' +
                    '            <div class="b-b-1 pt-3"></div>' +
                    '            <hr>' +
                    '            <div class="more-info pt-2" style="margin-bottom:-10px;">' +
                    '                <a class="font-weight-bold font-xs btn-block text-muted small buttonAgend" href="#dadosAgendamento" data-toggle="modal" id="buttonAgend_[ID]">Ver mais <i class="fa fa-angle-right float-right font-lg"></i></a>' +
                    '            </div>' +
                    '        </div>' +
                    '    </div>' +
                    '</div>' ;

    return gethtml;
}

function listaEquipamentosAgendados(Retorno)
{
    //console.log(Retorno);
    if(Retorno.status)
    {           
        var lista  = '<table id="table_id" class="table table-striped table-bordered">';
            lista += '  <thead>';
            lista += '      <tr>';
            lista += '          <th>Marca</th>';
            lista += '          <th>Tipo</th>';
            lista += '          <th>Modelo</th>';
            lista += '      </tr>';
            lista += '  </thead>';
            lista += '  <tbody>';

            for(var p = 0; p < Retorno.ListaAgendamentos[0].Equipamento.length; p++)
            {
                lista += '<tr id="rowid'+p+'">';
                lista += '  <td> ' + Retorno.ListaAgendamentos[0].Equipamento[p].marca + ' </td>';
                lista += '  <td> ' + Retorno.ListaAgendamentos[0].Equipamento[p].tipo + ' </td>';
                lista += '  <td> ' + Retorno.ListaAgendamentos[0].Equipamento[p].modelo + ' </td>';
                lista += '</tr>';
            }

            lista += '  </tbody>';
            lista += '</table>';

            jQuery("#listaEquipamentosAgendados").html(lista);
            jQuery("#table_id").DataTable({
                keys: false,
                responsive: true,
                "lengthMenu": [[7,15,30,50,100],[7,15,30,50,100]],
                "paging" : false,
                "bFilter": false,
                "bInfo"  : false
            });    
    }
}


function aprovar_cancelar_agendamento(cancelar = false)
{
    var session                 = getSessionStorage("AcessoColaborador");
    var cod_resp_alteracao      = session.cod_colaborador;
    var cod_colaborador         = session.cod_colaborador;
    var cod_agendamento         = jQuery("#cod_agendamento").val();
    var status_agendamento      = (cancelar) ? 4 : null;   

    var dados = {
        cod_colaborador        : cod_colaborador,
        cod_status_agendamento : status_agendamento,
        cod_resp_alteracao     : cod_resp_alteracao,
        cod_agendamento        : cod_agendamento
    }

    consultaWS("agendamento", "update", dados, function(Retorno)
    {
        if(Retorno.status)
        {
            Notificar(Retorno.msg);

            setTimeout(function(){
                jQuery('#dadosAgendamento').modal('toggle');

                const listIDS = [{id : "agendado"}, 
                                 {id : "aguardando_retirada"},
                                 {id : "aguardando_entrega"},
                                 {id : "entrega_confirmada"},
                                 {id : "solicitacao_cancelada"},
                                 {id : "solicitacao_em_atraso"}];

                limpar_div(listIDS);
                listaAgendamentos(false);
            },1000);
        }
        else
        {
            Notificar(Retorno.erro);
        }
    });
}