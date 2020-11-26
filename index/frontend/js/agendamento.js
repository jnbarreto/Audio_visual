
class solicitantes
{
    constructor(lista)
    {
        this.solicitante = lista
    }
}

jQuery(document).ready(function(){
    jQuery("#horario_de_ate").attr("disabled", "true");
    jQuery("#colaborador").attr("disabled", "true");
    jQuery("#funcional").attr("disabled", "true");
    jQuery("#equipamento").attr("disabled", "true");
    jQuery("#cod_equipamento").attr("disabled", "true");
    jQuery("#button_cod_equipamento").attr("disabled", "true");

    jQuery("#agendado_para").change(function(){

        var data = jQuery("#agendado_para").val();
        var dados = {
            data : data
        }

        consultaWS("agendamento", "getHorario", dados, carregaListaHora, false);
    });

    jQuery("#horario_de_ate").change(function(){

        var data = jQuery("#agendado_para").val();
        var hora = jQuery("#horario_de_ate").val();
        var data_hora = data + "|" + hora.split(' - ')[0];

        var dados = {
            data_hora : data_hora
        }

        consultaWS("agendamento", "verificaHorario", dados, verificaHorarioSel, false);

    });

    jQuery("#button_cod_equipamento").click(function(){
        carregaListaEquipamentos();
    });

    var session   = getSessionStorage("AcessoColaborador");
    listaSolicitantes(session);
});

function carregaListaHora(Retorno)
{
    if(!Retorno.status)
    {
        Notificar(Retorno.erro);
        jQuery("#horario_de_ate").attr("disabled", "true");

        jQuery("#horario_de_ate").val("0");
        jQuery('#horario_de_ate option:not(:selected)').remove();
        
        jQuery("#agendado_para").focus();
    }
    else
    {
        jQuery("#horario_de_ate").removeAttr("disabled");
        jQuery("#horario_de_ate").focus();
    
        var optionHorario  = '<option value="0"> Selecione </option>';
        for(var h = 0; h < Retorno.ListaHorarios.length; h++)
        {
            optionHorario += '<option value="'+Retorno.ListaHorarios[h].horario_de +' - '+ Retorno.ListaHorarios[h].horario_ate+'">';
            optionHorario += Retorno.ListaHorarios[h].horario_de+' - '+Retorno.ListaHorarios[h].horario_ate+'</option>';
        }
        jQuery("#horario_de_ate").html(optionHorario);
    }
}

function verificaHorarioSel(Retorno)
{
    if(!Retorno.status)
    {
        Notificar(Retorno.erro);
        jQuery("#button_cod_equipamento").attr("disabled", "true");

        jQuery("#cod_equipamento").val("");
        jQuery("#equipamento").val("");

        jQuery("#horario_de_ate").focus();
    }
    else
    {
        jQuery("#button_cod_equipamento").removeAttr("disabled");

        jQuery("#button_cod_equipamento").click();
    }
}

function carregaListaEquipamentos(Retorno)
{
    var dados = {
        agendamento : true
    }

    consultaWS("equipamentos", "select", dados, function(Retorno)
    {

        jQuery("#ListaEquipamentosAgendamento-popup").remove();
        var Listagem = '<table id="ListaEquipamentosAgendamento-popup" class="table table-striped table-bordered dt-responsive nowrap" cellspacing="0" width="100%">';
        Listagem += '   <thead>';
        Listagem += '      <tr>';
        Listagem += '         <th>Codigo</th>';
        Listagem += '         <th>Marca</th>';
        Listagem += '         <th>Tipo</th>';
        Listagem += '         <th>Modelo</th>';
        Listagem += '         <th style="width:100px;"></th>';
        Listagem += '      </tr>';
        Listagem += '   </thead>';
        Listagem += '<tbody>';

        for(var i=0;i<Retorno.ListaEquipamentos.length; i++)
        {
            var mtm = Retorno.ListaEquipamentos[i].marca + " - " +  Retorno.ListaEquipamentos[i].tipo + " - " + Retorno.ListaEquipamentos[i].modelo;

            Listagem += '<tr>';
            Listagem += '   <td>'+Retorno.ListaEquipamentos[i].cod_equipamento+'</td>';
            Listagem += '   <td>'+Retorno.ListaEquipamentos[i].marca+'</td>';
            Listagem += '   <td>'+Retorno.ListaEquipamentos[i].tipo+'</td>';
            Listagem += '   <td>'+Retorno.ListaEquipamentos[i].modelo+'</td>';
            Listagem += '   <td><button type="button" data-dismiss="modal" class="btn btn-primary" onclick="setVal(\'cod_equipamento\', \'equipamento\',\''+Retorno.ListaEquipamentos[i].cod_equipamento+'\',\''+mtm+'\')">Selecionar</button></td>';
            Listagem += '</tr>';
        }
        Listagem += '</tbody>';
        Listagem += '</table>';

        jQuery("#ListaEquipamentosAgendamento").html(Listagem);

        jQuery('#ListaEquipamentosAgendamento-popup').DataTable({
            keys: false,
            responsive: true,
            "lengthMenu": [[4],[4]],		  
            "bLengthChange": false
        });

    }, false);
}

function salvarAgendamento()
{
    var session         = getSessionStorage("AcessoColaborador");

    var cod_agendado_por    = session.cod_colaborador;
    var cod_colaborador     = jQuery("#cod_colaborador").val(); //Solicitante do agendamento (Professor ou Administrador)
    var agendado_para       = jQuery("#agendado_para").val(); // Data do agendamento

    var horario             = jQuery("#horario_de_ate").val().split(' - ');
    var horario_de          = horario[0];
    var horario_ate         = horario[1];

    var Equipamento = [];
    Equipamento.push({
        cod_equipamento : jQuery("#cod_equipamento").val()
    });

    var dados = {
        cod_colaborador : cod_colaborador,
        cod_agendado_por: cod_agendado_por,
        agendado_para   : agendado_para,
        horario_de      : horario_de,
        horario_ate     : horario_ate,
        Equipamento     : Equipamento
    }

    consultaWS("agendamento", "insert", dados, function(Retorno){
        if(Retorno.status)
        {
            Notificar(Retorno.msg);
        }
        else
        {
            Notificar(Retorno.erro);
        }
    });
}

function listaSolicitantes(session)
{
    let s = null;

    var dados = {
        cod_colaborador : session.cod_colaborador
    }

    consultaWS("colaboradores", "getListaSolicitante", dados, function(Retorno){
        if(Retorno.status)
        {
            s = new solicitantes(Retorno.ListaColaboradores);

            var optionSolic  = "<option value='0'> Selecione </option>";
            for(var os = 0; os < Retorno.ListaColaboradores.length; os++)
            {
                optionSolic += "<option value="+Retorno.ListaColaboradores[os].cod_colaborador+">";
                optionSolic += Retorno.ListaColaboradores[os].nome_colaborador+"</option>";
            }
            jQuery("#cod_colaborador").html(optionSolic);
        }
        else
        {
            Notificar(Retorno.erro);
        }
    }, false);

    jQuery("#cod_colaborador").change(function(){
        jQuery("#funcional").val("");
        for(var os = 0; os < s.solicitante.length; os++)
        {
            var cod = jQuery("#cod_colaborador").val();
            if(cod == s.solicitante[os].cod_colaborador)
            {
                jQuery("#funcional").val(s.solicitante[os].funcional);
            }
        }
    });
}

function equipamentos_reservados()
{
    var session         = getSessionStorage("AcessoColaborador");

    var dados = {
        cod_colaborador : session.cod_colaborador
    }

    console.log(JSON.stringify(dados));

    consultaWS("agendamento", "getAgendColab", dados, function(Retorno)
    {
        var Listagem = '<table id="table_id" class="table table-striped table-bordered dt-responsive nowrap" cellspacing="0" width="100%">';
        Listagem += '   <thead>';
        Listagem += '      <tr>';
        Listagem += '         <th>Marca</th>';
        Listagem += '         <th>Tipo</th>';
        Listagem += '         <th>Status</th>';
        Listagem += '         <th style="width:400px;"></th>';
        Listagem += '      </tr>';
        Listagem += '   </thead>';
        Listagem += '<tbody>';

        for(var i = 0 ; i < Retorno.ListaAgendamentos.length; i++)
        {
            var cod_status_agendamento = Retorno.ListaAgendamentos[i].cod_status_agendamento;
            var estilo   = '';
            var valuenow = '';
            var cor      = '';

            switch(cod_status_agendamento)
            {
                case 1:
                    estilo   = "width: 16%";
                    valuenow = "16";
                    cor      = "bg-success";
                break;

                case 2:
                    estilo = "width: 32%";
                    valuenow = "32";
                    cor      = "bg-warning";
                break;

                case 3:
                    estilo = "width: 48%";
                    valuenow = "48";
                    cor      = "bg-warning";
                break;

                case 4:
                    estilo = "width: 64%";
                    valuenow = "64";
                    cor      = "bg-info";
                break;

                case 5:
                    estilo = "width: 80%";
                    valuenow = "80";
                    cor      = "bg-danger";
                break;

                case 6:
                    estilo = "width: 100%";
                    valuenow = "100";
                    cor      = "bg-success";
                break;
            }


            for(var j = 0; j < Retorno.ListaAgendamentos[i].Equipamento.length; j++)
            {
                Listagem += '<tr>';
                Listagem += '   <td>'+Retorno.ListaAgendamentos[i].Equipamento[j].marca.toUpperCase()+'</td>';
                Listagem += '   <td>'+Retorno.ListaAgendamentos[i].Equipamento[j].tipo.toUpperCase()+'</td>';
                Listagem += '   <td>'+Retorno.ListaAgendamentos[i].Status.tipo_status_agendamento.toUpperCase()+'</td>';
                Listagem += '   <td>' + 
                            '       <div class="progress mb-2">' +
                            '           <div class="progress-bar '+ cor +' progress-bar-striped progress-bar-animated" role="progressbar" style="'+ estilo +'" aria-valuenow="'+ valuenow +'" aria-valuemin="0" aria-valuemax="100">'+valuenow+'%</div>' +
                            '       </div>' +
                            '   </td>';

                Listagem += '</tr>';
            }
        }
        Listagem += '</tbody>';
        Listagem += '</table>';

        jQuery("#ListaAgendados").html(Listagem);

        jQuery('#table_id').DataTable({
            keys: false,
            responsive: true,
            "lengthMenu": [[4],[4]],		  
            "bLengthChange": false
        });

    }, false);
}