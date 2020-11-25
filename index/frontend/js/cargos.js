function listaCargos(Retorno)
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

            for(var p = 0; p < Retorno.ListaCargos.length; p++)
            {
                // var ativar_inativar = false;
                // var title = "";
                // var msg   = "";
                //var status_ativo = 'Inativar';

                var cod = Retorno.ListaCargos[p].cod_cargo;

                lista += '<tr id="rowid'+cod+'">';
                lista += '  <td> ' + Retorno.ListaCargos[p].cod_cargo + ' </td>';
                lista += '  <td> ' + Retorno.ListaCargos[p].tipo_cargo.toUpperCase() + ' </td>';
                lista += '  <td> ' + Retorno.ListaCargos[p].descricao_cargo.toUpperCase() + ' </td>';
                
                // Botao
                lista += '  <td width="120px">'  
                lista += '      <div class="btn-group" role="group">';
                lista += '          <button id="btnGroupDrop1" type="button" class="btn btn-success dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">';
                lista += '              Alterar';
                lista += '          </button>';
                lista += '          <div class="dropdown-menu" aria-labelledby="btnGroupDrop1">';
                lista += '              <a class="dropdown-item" onclick="window.location.href=\'/frontend/pages/cargos/form.html?id='+cod+'\'">Alterar</a>';
                //lista += '              <a class="dropdown-item" onclick="confirmaInativacao(\''+title+'\', \''+msg+'\', '+cod+', '+ativar_inativar+', inativarReativarColaborador)">'+status_ativo+'</a>';
                lista += '          </div>';
                lista += '      </div>';
                lista += '  </td>';
                // Botao

                lista += '</tr>';
            }

            lista += '  </tbody>';
            lista += '</table>';

            jQuery("#ListaCargos").html(lista);
            jQuery("#table_id").DataTable({
                keys: false,
                responsive: true,
                "lengthMenu": [[7,15,30,50,100],[7,15,30,50,100]]
            });    
    }
}


function getCargos(cod  = null)
{
    var dados = {
        cod_cargo : cod
    }

    console.log(JSON.stringify(dados));
    consultaWS('cargos', 'get', dados, function(Retorno){
        console.log(Retorno);

        if(Retorno.status)
        {   
            var c = 0;

            var cod_cargo       = Retorno.ListaCargos[c].cod_cargo;
            var tipo_cargo      = Retorno.ListaCargos[c].tipo_cargo;
            var descricao_cargo = Retorno.ListaCargos[c].descricao_cargo;

            jQuery("#alteracao").val('S');

            jQuery("#cod_cargo").val(cod_cargo);
            jQuery("#tipo_cargo").val(tipo_cargo);
            jQuery("#descricao_cargo").val(descricao_cargo);
        }
        else
        {
            Notificar(Retorno.erro);
        }
    });
}


function insertCargos()
{
    var cod_cargo       = jQuery.trim(jQuery("#cod_cargo").val());
    var tipo_cargo      = jQuery.trim(jQuery("#tipo_cargo").val());
    var descricao_cargo = jQuery.trim(jQuery("#descricao_cargo").val());

    var dados = {
        cod_cargo       : cod_cargo,
        tipo_cargo      : tipo_cargo,
        descricao_cargo : descricao_cargo
    }

    console.log(JSON.stringify(dados));
    var metodo = 'insert';
    if(jQuery("#alteracao").val() == 'S')
    {
        metodo = 'update';
    }

    consultaWS('cargos', metodo, dados, function(Retorno){
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