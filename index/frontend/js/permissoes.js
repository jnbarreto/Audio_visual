function listaPermissoes(Retorno)
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

            for(var p = 0; p < Retorno.ListaPermissoes.length; p++)
            {
                // var ativar_inativar = false;
                // var title = "";
                // var msg   = "";
                // var status_ativo = 'Inativar';

                var cod = Retorno.ListaPermissoes[p].cod_permissao;

                lista += '<tr id="rowid'+cod+'">';
                lista += '  <td> ' + Retorno.ListaPermissoes[p].cod_permissao + ' </td>';
                lista += '  <td> ' + Retorno.ListaPermissoes[p].tipo_permissao.toUpperCase() + ' </td>';
                lista += '  <td> ' + Retorno.ListaPermissoes[p].descricao_permissao.toUpperCase() + ' </td>';
                
                // Botao
                lista += '  <td width="120px">'  
                lista += '      <div class="btn-group" role="group">';
                lista += '          <button id="btnGroupDrop1" type="button" class="btn btn-success dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">';
                lista += '              Alterar';
                lista += '          </button>';
                lista += '          <div class="dropdown-menu" aria-labelledby="btnGroupDrop1">';
                lista += '              <a class="dropdown-item" onclick="window.location.href=\'/frontend/pages/permissoes/form.html?id='+cod+'\'">Alterar</a>';
                //lista += '              <a class="dropdown-item" onclick="confirmaInativacao(\''+title+'\', \''+msg+'\', '+cod+', '+ativar_inativar+', inativarReativarColaborador)">'+status_ativo+'</a>';
                lista += '          </div>';
                lista += '      </div>';
                lista += '  </td>';
                // Botao

                lista += '</tr>';
            }

            lista += '  </tbody>';
            lista += '</table>';

            jQuery("#ListaPermissoes").html(lista);
            jQuery("#table_id").DataTable({
                keys: false,
                responsive: true,
                "lengthMenu": [[7,15,30,50,100],[7,15,30,50,100]]
            });    
    }
}

function getPermissoes(cod  = null)
{
    var dados = {
        cod_permissao : cod
    }

    console.log(JSON.stringify(dados));
    consultaWS('permissoes', 'get', dados, function(Retorno){
        console.log(Retorno);

        if(Retorno.status)
        {   
            var p = 0;

            var cod_permissao       = Retorno.ListaPermissoes[p].cod_permissao;
            var tipo_permissao      = Retorno.ListaPermissoes[p].tipo_permissao;
            var descricao_permissao = Retorno.ListaPermissoes[p].descricao_permissao;

            jQuery("#alteracao").val('S');

            jQuery("#cod_permissao").val(cod_permissao);
            jQuery("#tipo_permissao").val(tipo_permissao);
            jQuery("#descricao_permissao").val(descricao_permissao);
        }
        else
        {
            Notificar(Retorno.erro);
        }
    });
}

function insertPermissoes()
{
    var cod_permissao       = jQuery.trim(jQuery("#cod_permissao").val());
    var tipo_permissao      = jQuery.trim(jQuery("#tipo_permissao").val());
    var descricao_permissao = jQuery.trim(jQuery("#descricao_permissao").val());

    var dados = {
        cod_permissao       : cod_permissao,
        tipo_permissao      : tipo_permissao,
        descricao_permissao : descricao_permissao
    }

    console.log(JSON.stringify(dados));
    var metodo = 'insert';
    if(jQuery("#alteracao").val() == 'S')
    {
        metodo = 'update';
    }

    consultaWS('permissoes', metodo, dados, function(Retorno){
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