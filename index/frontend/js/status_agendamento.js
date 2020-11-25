function ListaStatusAgendamento(Retorno)
{
    console.log(Retorno);
    if(Retorno.status)
    {           
        var lista  = '<table id="table_id" class="table table-striped table-bordered">';
            lista += '  <thead>';
            lista += '      <tr>';
            lista += '          <th>Cod</th>';
            lista += '          <th>Tipo</th>';
            lista += '          <th>Descrição</th>';
            lista += '          <th> </th>';
            lista += '      </tr>';
            lista += '  </thead>';
            lista += '  <tbody>';

            for(var p = 0; p < Retorno.ListaStatusAgendamento.length; p++)
            {
                // var ativar_inativar = false;
                // var title = "";
                // var msg   = "";
                //var status_ativo = 'Inativar';

                var cod = Retorno.ListaStatusAgendamento[p].cod_status_agendamento;

                lista += '<tr id="rowid'+cod+'">';
                lista += '  <td> ' + Retorno.ListaStatusAgendamento[p].cod_status_agendamento + ' </td>';
                lista += '  <td> ' + Retorno.ListaStatusAgendamento[p].tipo_status_agendamento.toUpperCase() + ' </td>';
                lista += '  <td> ' + Retorno.ListaStatusAgendamento[p].descricao_status_agendamento.toUpperCase() + ' </td>';
                
                // Botao
                lista += '  <td width="120px">'  
                lista += '      <div class="btn-group" role="group">';
                lista += '          <button id="btnGroupDrop1" type="button" class="btn btn-success dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">';
                lista += '              Alterar';
                lista += '          </button>';
                lista += '          <div class="dropdown-menu" aria-labelledby="btnGroupDrop1">';
                lista += '              <a class="dropdown-item" onclick="window.location.href=\'/frontend/pages/status_agendamento/form.html?id='+cod+'\'">Alterar</a>';
                //lista += '              <a class="dropdown-item" onclick="confirmaInativacao(\''+title+'\', \''+msg+'\', '+cod+', '+ativar_inativar+', inativarReativarColaborador)">'+status_ativo+'</a>';
                lista += '          </div>';
                lista += '      </div>';
                lista += '  </td>';
                // Botao

                lista += '</tr>';
            }

            lista += '  </tbody>';
            lista += '</table>';

            jQuery("#ListaStatusAgendamento").html(lista);
            jQuery("#table_id").DataTable({
                keys: false,
                responsive: true,
                "lengthMenu": [[7,15,30,50,100],[7,15,30,50,100]]
            });    
    }
}


function getStatusAgendamento(cod  = null)
{
    var dados = {
        cod_status_agendamento : cod
    }

    console.log(JSON.stringify(dados));
    consultaWS('statusagendamento', 'get', dados, function(Retorno){
        console.log(Retorno);

        if(Retorno.status)
        {   
            var c = 0;

            var cod_status_agendamento       = Retorno.ListaStatusAgendamento[c].cod_status_agendamento;
            var tipo_status_agendamento      = Retorno.ListaStatusAgendamento[c].tipo_status_agendamento;
            var descricao_status_agendamento = Retorno.ListaStatusAgendamento[c].descricao_status_agendamento;

            jQuery("#alteracao").val('S');

            jQuery("#cod_status_agendamento").val(cod_status_agendamento);
            jQuery("#tipo_status_agendamento").val(tipo_status_agendamento);
            jQuery("#descricao_status_agendamento").val(descricao_status_agendamento);
        }
        else
        {
            Notificar(Retorno.erro);
        }
    });
}


function insertStatusAgendamento()
{
    var cod_status_agendamento       = jQuery.trim(jQuery("#cod_status_agendamento").val());
    var tipo_status_agendamento      = jQuery.trim(jQuery("#tipo_status_agendamento").val());
    var descricao_status_agendamento = jQuery.trim(jQuery("#descricao_status_agendamento").val());

    var dados = {
        cod_status_agendamento       : cod_status_agendamento,
        tipo_status_agendamento      : tipo_status_agendamento,
        descricao_status_agendamento : descricao_status_agendamento
    }

    console.log(JSON.stringify(dados));
    var metodo = 'insert';
    if(jQuery("#alteracao").val() == 'S')
    {
        metodo = 'update';
    }

    consultaWS('statusagendamento', metodo, dados, function(Retorno){
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