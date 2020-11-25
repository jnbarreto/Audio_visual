class Mensagem{
    constructor()
    {
        this.title_reativar = "Reativação de Equipamento";
        this.title_inativar = "Inativação de Equipamento";
        this.msg_reativar   = "Deseja realmente reativar o equipamento ";
        this.msg_inativar   = "Deseja realmente inativar o equipamento ";
    }
}

function listaEquipamentos(Retorno)
{
    console.log(Retorno);
    if(Retorno.status)
    {           
        const mensagem = new Mensagem();
        var lista  = '<table id="table_id" class="table table-striped table-bordered">';
            lista += '  <thead>';
            lista += '      <tr>';
            lista += '          <th>Cod</th>';
            lista += '          <th>Patrimônio</th>';
            lista += '          <th>Num Controle</th>';
            lista += '          <th>Marca</th>';
            lista += '          <th>Tipo</th>';
            lista += '          <th>Modelo</th>';
            lista += '          <th> </th>';
            lista += '      </tr>';
            lista += '  </thead>';
            lista += '  <tbody>';

            for(var p = 0; p < Retorno.ListaEquipamentos.length; p++)
            {
                var ativar_inativar = false;
                var title = "";
                var msg   = "";
                var status_ativo = 'Inativar';

                var status_ativo = Retorno.ListaEquipamentos[p].status_ativo;
                if(status_ativo == 'S')
                {
                    status_ativo = 'Inativar';
                    title = mensagem.title_inativar;
                    msg   = mensagem.msg_inativar + Retorno.ListaEquipamentos[p].marca + " - " + Retorno.ListaEquipamentos[p].tipo + " - " + Retorno.ListaEquipamentos[p].modelo + "?";
                }
                else
                {
                    status_ativo = 'Ativar';
                    title = mensagem.title_reativar;
                    msg   = mensagem.msg_reativar + Retorno.ListaEquipamentos[p].marca + " - " + Retorno.ListaEquipamentos[p].tipo + " - " + Retorno.ListaEquipamentos[p].modelo + "?";
                    ativar_inativar = true;
                }

                var cod = Retorno.ListaEquipamentos[p].cod_equipamento;

                lista += '<tr id="rowid'+cod+'">';
                lista += '  <td> ' + Retorno.ListaEquipamentos[p].cod_equipamento + ' </td>';
                lista += '  <td> ' + Retorno.ListaEquipamentos[p].cod_patrimonio + ' </td>';
                lista += '  <td> ' + Retorno.ListaEquipamentos[p].num_controle + ' </td>';
                lista += '  <td> ' + Retorno.ListaEquipamentos[p].marca.toUpperCase() + ' </td>';
                lista += '  <td> ' + Retorno.ListaEquipamentos[p].tipo.toUpperCase() + ' </td>';
                lista += '  <td> ' + Retorno.ListaEquipamentos[p].modelo.toUpperCase() + ' </td>';
                
                // Botao
                lista += '  <td width="120px">'  
                lista += '      <div class="btn-group" role="group">';
                lista += '          <button id="btnGroupDrop1" type="button" class="btn btn-success dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">';
                lista += '              Alterar';
                lista += '          </button>';
                lista += '          <div class="dropdown-menu" aria-labelledby="btnGroupDrop1">';
                lista += '              <a class="dropdown-item" onclick="window.location.href=\'/frontend/pages/equipamentos/form.html?id='+cod+'\'">Alterar</a>';
                lista += '              <a class="dropdown-item" onclick="confirmaInativacao(\''+title+'\', \''+msg+'\', '+cod+', '+ativar_inativar+', inativarReativarEquipamento)">'+status_ativo+'</a>';
                lista += '          </div>';
                lista += '      </div>';
                lista += '  </td>';
                // Botao

                lista += '</tr>';
            }

            lista += '  </tbody>';
            lista += '</table>';

            jQuery("#ListaEquipamentos").html(lista);
            jQuery("#table_id").DataTable({
                keys: false,
                responsive: true,
                "lengthMenu": [[7,15,30,50,100],[7,15,30,50,100]]
            });    
    }
}

function getEquipamentos(cod  = null)
{
    var dados = {
        cod_equipamento : cod
    }

    console.log(JSON.stringify(dados));
    consultaWS('equipamentos', 'get', dados, function(Retorno){
        console.log(Retorno);

        if(Retorno.status)
        {   
            var p = 0;

            var cod_equipamento     = Retorno.ListaEquipamentos[p].cod_equipamento;
            var cod_patrimonio      = Retorno.ListaEquipamentos[p].cod_patrimonio;
            var num_controle        = Retorno.ListaEquipamentos[p].num_controle;
            var marca               = Retorno.ListaEquipamentos[p].marca;
            var tipo                = Retorno.ListaEquipamentos[p].tipo;
            var modelo              = Retorno.ListaEquipamentos[p].modelo;

            jQuery("#alteracao").val('S');

            jQuery("#cod_equipamento").val(cod_equipamento);
            jQuery("#cod_patrimonio").val(cod_patrimonio);
            jQuery("#num_controle").val(num_controle);
            jQuery("#marca").val(marca);
            jQuery("#tipo").val(tipo);
            jQuery("#modelo").val(modelo);
        }
        else
        {
            Notificar(Retorno.erro);
        }
    });
}

function insertEquipamentos()
{
    var cod_equipamento     = jQuery.trim(jQuery("#cod_equipamento").val());
    var cod_patrimonio      = jQuery.trim(jQuery("#cod_patrimonio").val());
    var num_controle        = jQuery.trim(jQuery("#num_controle").val());
    var marca               = jQuery.trim(jQuery("#marca").val());
    var tipo                = jQuery.trim(jQuery("#tipo").val());
    var modelo              = jQuery.trim(jQuery("#modelo").val());

    var dados = {
        cod_equipamento     : cod_equipamento,
        cod_patrimonio      : cod_patrimonio,
        num_controle        : num_controle,
        marca               : marca,
        tipo                : tipo,
        modelo              : modelo
    }

    console.log(JSON.stringify(dados));
    var metodo = 'insert';
    if(jQuery("#alteracao").val() == 'S')
    {
        metodo = 'update';
    }

    consultaWS('equipamentos', metodo, dados, function(Retorno){
        if(Retorno.status)
        {
            Notificar(Retorno.msg);
            setTimeout(function(){
                IrPara("/frontend/pages/equipamentos");
            },2000);
        }
        else
        {
            Notificar(Retorno.erro);
        }
    });
}

function inativarReativarEquipamento(cod, ativar_inativar)
{
    var status_ativo = (ativar_inativar) ? status_ativo = 'S' : status_ativo = 'N';

    var dados = {
        cod_equipamento : cod,
        status_ativo : status_ativo    
    }

    consultaWS('equipamentos', 'inativarReativar', dados, function(Retorno){
        if(Retorno.status)
        {
            Notificar(Retorno.msg);

            setTimeout(function(){
                consultaWS('equipamentos', 'select', null, listaEquipamentos);
            },1000);
        }
        else
        {
            Notificar(Retorno.erro);
        }
    });
}

function listaDadosXLS(obj)
{    
    var objDados = objConvert(obj);
    var lista  = '<table id="table_id_xls" class="table table-striped table-bordered">';
            lista += '  <thead>';
            lista += '      <tr>';
            lista += '          <th>Patrimônio</th>';
            lista += '          <th>Num Controle</th>';
            lista += '          <th>Marca</th>';
            lista += '          <th>Tipo</th>';
            lista += '          <th>Modelo</th>';
            lista += '      </tr>';
            lista += '  </thead>';
            lista += '  <tbody>';

            for(var p = 0; p < objDados.length; p++)
            {
                var cod = objDados[p].cod_patrimonio;

                lista += '<tr id="rowid'+cod+'">';
                lista += '  <td> ' + objDados[p].cod_patrimonio + ' </td>';
                lista += '  <td> ' + objDados[p].num_controle + ' </td>';
                lista += '  <td> ' + objDados[p].marca + ' </td>';
                lista += '  <td> ' + objDados[p].tipo + ' </td>';
                lista += '  <td> ' + objDados[p].modelo + ' </td>';
                lista += '</tr>';
            }

            lista += '  </tbody>';
            lista += '</table>';

            jQuery("#ListaXLS").html(lista);
            jQuery("#table_id_xls").DataTable({
                keys: false,
                responsive: true,
                "lengthMenu": [[7,15,30,50,100],[7,15,30,50,100]],
                pageLength : 100,
                "paging":   false,
                "info":     false,
                "bFilter": false
            });    
    
}

function objConvert(obj)
{
    var dadosArray = [];

    var cod_patrimonio  = obj.dados[0][0];
    var num_controle    = obj.dados[0][1];
    var marca           = obj.dados[0][2];
    var tipo            = obj.dados[0][3];
    var modelo          = obj.dados[0][4];

    for (let i = 1; i < obj.dados.length; i++) 
    {
        if (obj.dados[i].length > 0) 
        {
            var dados = obj.dados[i];

            dadosArray.push({
                [cod_patrimonio] : dados[0],
                [num_controle] : dados[1],
                [marca] : dados[2],
                [tipo] : dados[3],
                [modelo] : dados[4]
            });
        }
    }

    return dadosArray;
}

