jQuery(document).ready(function(){

    filtraRelatorio();
    listaColaborador();

    jQuery("#gerar").click(function(){

        var session          = getSessionStorage("AcessoColaborador");
        var processado_por   = session.nome_colaborador;

        var dados = {
            cod_colaborador : jQuery("#cod_colaborador").val(),
            mes_de          : jQuery("#mes_de").val(),
            mes_ate         : jQuery("#mes_ate").val(),
            processado_por  : processado_por
        }

        downloadfile(dados);
    });

    jQuery("#filtrar").click(function(){
        var dados = {
            cod_colaborador : jQuery("#cod_colaborador").val(),
            mes_de          : jQuery("#mes_de").val(),
            mes_ate         : jQuery("#mes_ate").val()
        }

        filtraRelatorio(dados);
    });

    jQuery("#mes_de").change(function(){
        if(jQuery("#mes_de").val() != "")
        {
            jQuery('#mes_ate').focus();
        }
        else
        {
            jQuery('#mes_de').focus();
        }
    });
});

function listaRelatorio(Retorno)
{
    console.log(Retorno);
    if(Retorno.status)
    {           
        var lista  = '<table id="table_id" class="table table-striped table-bordered">';
            lista += '  <thead>';
            lista += '      <tr>';
            lista += '          <th>Colaborador</th>';
            lista += '          <th>RF</th>';
            lista += '          <th>Agend. Em</th>';
            lista += '          <th>Hora Agend.</th>';
            lista += '          <th>Agend. Para Dia</th>';
            lista += '          <th>Horario De</th>';
            lista += '          <th>Horario Ate</th>';
            lista += '          <th>Tipo Equipamento</th>';
            lista += '      </tr>';
            lista += '  </thead>';
            lista += '  <tbody>';

            for(var p = 0; p < Retorno.ListaRelatorio.length; p++)
            {
                // var ativar_inativar = false;
                // var title = "";
                // var msg   = "";
                // var status_ativo = 'Inativar';

                lista += '<tr id="rowid'+p+'">';
                lista += '  <td> ' + Retorno.ListaRelatorio[p].nome_colaborador.toUpperCase() + ' </td>';
                lista += '  <td> ' + Retorno.ListaRelatorio[p].funcional.toUpperCase() + ' </td>';
                lista += '  <td> ' + Retorno.ListaRelatorio[p].data_reserva + ' </td>';
                lista += '  <td> ' + Retorno.ListaRelatorio[p].hora_reserva + ' </td>';
                lista += '  <td> ' + Retorno.ListaRelatorio[p].agendado_para + ' </td>';
                lista += '  <td> ' + Retorno.ListaRelatorio[p].horario_de + ' </td>';
                lista += '  <td> ' + Retorno.ListaRelatorio[p].horario_ate + ' </td>';
                lista += '  <td> ' + Retorno.ListaRelatorio[p].tipo.toUpperCase() + ' </td>';

                lista += '</tr>';
            }

            lista += '  </tbody>';
            lista += '</table>';

            jQuery("#ListaRelat").html(lista);
            jQuery("#table_id").DataTable({
                keys: false,
                responsive: true,
                "lengthMenu": [[7,15,30,50,100],[7,15,30,50,100]]
            });    
    }
    else
    {
        Notificar(Retorno.erro);
    }
}


function listaColaborador()
{
    consultaWS("colaboradores", "select", null, function(Retorno){
        if(Retorno.status)
        {
            var optionSolic  = "<option value=''> Todos </option>";
            for(var os = 0; os < Retorno.ListaColaboradores.length; os++)
            {
                optionSolic += "<option value="+Retorno.ListaColaboradores[os].cod_colaborador+">";
                optionSolic += Retorno.ListaColaboradores[os].nome_colaborador+" - " + Retorno.ListaColaboradores[os].funcional + "</option>";
            }
            jQuery("#cod_colaborador").html(optionSolic);
        }
        else
        {
            Notificar(Retorno.erro);
        }
    }, false);
}

function filtraRelatorio(dados =  null)
{
    consultaWS('relatorio', 'select', dados, listaRelatorio);
}

function downloadfile(dados)
{

    consultaWS("relatorio", "relatorio", dados, function(Retorno){
        console.log(Retorno);
        if(Retorno.status)
        {
            var link = Retorno.donwload[0].link;
            IrPara(link);
        }
        else
        {
            Notificar(Retorno.erro);
        }
    });
}