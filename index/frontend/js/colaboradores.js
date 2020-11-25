class Mensagem{
    constructor()
    {
        this.title_reativar = "Reativação de Colaborador";
        this.title_inativar = "Inativação de Colaborador";
        this.msg_reativar   = "Deseja realmente reativar o colaborador ";
        this.msg_inativar   = "Deseja realmente inativar o colaborador ";
    }
}

function listaColaboradores(Retorno)
{
    //console.log(Retorno);
    if(Retorno.status)
    {   
        const mensagem = new Mensagem();
        var lista  = '<table id="table_id" class="table table-striped table-bordered">';
            lista += '  <thead>';
            lista += '      <tr>';
            lista += '          <th>Cod</th>';
            lista += '          <th>Nome</th>';
            lista += '          <th>Contato</th>';
            lista += '          <th>E-mail</th>';
            lista += '          <th>RF</th>';
            lista += '          <th> </th>';
            lista += '      </tr>';
            lista += '  </thead>';
            lista += '  <tbody>';

            for(var c = 0; c < Retorno.ListaColaboradores.length; c++)
            {
                var ativar_inativar = false;
                var title = "";
                var msg   = "";

                var cod = Retorno.ListaColaboradores[c].cod_colaborador;

                var status_ativo = Retorno.ListaColaboradores[c].Acesso.status_ativo;
                if(status_ativo == 'S')
                {
                    status_ativo = 'Inativar';
                    title = mensagem.title_inativar;
                    msg   = mensagem.msg_inativar + Retorno.ListaColaboradores[c].nome_colaborador+"?";
                }
                else
                {
                    status_ativo = 'Ativar';
                    title = mensagem.title_reativar;
                    msg   = mensagem.msg_reativar + Retorno.ListaColaboradores[c].nome_colaborador+"?";
                    ativar_inativar = true;
                }

                lista += '<tr id="rowid'+cod+'">';
                lista += '  <td> ' + Retorno.ListaColaboradores[c].cod_colaborador + ' </td>';
                lista += '  <td> ' + Retorno.ListaColaboradores[c].nome_colaborador.toUpperCase() + ' </td>';
                lista += '  <td> ' + Retorno.ListaColaboradores[c].contato + ' </td>';
                lista += '  <td> ' + Retorno.ListaColaboradores[c].email.toUpperCase() + ' </td>';
                lista += '  <td> ' + Retorno.ListaColaboradores[c].funcional.toUpperCase() + ' </td>';
                
                // Botao
                lista += '  <td width="120px">'  
                lista += '      <div class="btn-group" role="group">';
                lista += '          <button id="btnGroupDrop1" type="button" class="btn btn-success dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">';
                lista += '              Alterar';
                lista += '          </button>';
                lista += '          <div class="dropdown-menu" aria-labelledby="btnGroupDrop1">';
                lista += '              <a class="dropdown-item" onclick="window.location.href=\'/frontend/pages/usuarios/form.html?id='+cod+'\'">Alterar</a>';
                lista += '              <a class="dropdown-item" onclick="confirmaInativacao(\''+title+'\', \''+msg+'\', '+cod+', '+ativar_inativar+', inativarReativarColaborador)">'+status_ativo+'</a>';
                lista += '          </div>';
                lista += '      </div>';
                lista += '  </td>';
                // Botao

                lista += '</tr>';
            }

            lista += '  </tbody>';
            lista += '</table>';

            jQuery("#ListaUsuarios").html(lista);
            jQuery("#table_id").DataTable({
                keys: false,
                responsive: true,
                "lengthMenu": [[7,15,30,50,100],[7,15,30,50,100]]
            });    
    }
}

function getColaboradores(cod  = null)
{
    if( cod != null )
    {
        var dados = {
            cod_colaborador : cod
        }
    
        console.log(JSON.stringify(dados));
        consultaWS('colaboradores', 'get', dados, function(Retorno){
            console.log(Retorno);
    
            if(Retorno.status)
            {   
                var c = 0;
    
                //Dados Colaborador
                var nome_colaborador    = Retorno.ListaColaboradores[c].nome_colaborador;
                var email               = Retorno.ListaColaboradores[c].email;
                var funcional           = Retorno.ListaColaboradores[c].funcional;
                var contato             = Retorno.ListaColaboradores[c].contato;
                var cod_permissao       = Retorno.ListaColaboradores[c].cod_permissao;
                var cod_colaborador     = Retorno.ListaColaboradores[c].cod_colaborador;
                var cod_cargo           = Retorno.ListaColaboradores[c].cod_cargo;
    
                //Dados Acesso
                var login               = Retorno.ListaColaboradores[c].Acesso.login;
                var senha               = Retorno.ListaColaboradores[c].Acesso.senha;
                var cod_acesso          = Retorno.ListaColaboradores[c].Acesso.cod_acesso;
    
                jQuery("#alteracao").val("S");
    
                //Inserção de dados em campos input dados colaborador
                jQuery("#nome_colaborador").val(nome_colaborador);
                jQuery("#contato").val(contato);
                jQuery("#email").val(email);
                jQuery("#funcional").val(funcional);
                jQuery("#cod_colaborador").val(cod_colaborador);
    
                //Inserção de dados em campos input acesso
                jQuery("#login").val(login).prop("disabled", true);
                jQuery("#senha").val(senha);
                jQuery("#conf_senha").val(senha);
                jQuery("#cod_acesso").val(cod_acesso);
    
                var status_ativo = document.querySelector('.status_ativo');
                (Retorno.ListaColaboradores[c].Acesso.status_ativo == 'S') ? jQuery(status_ativo).trigger('click').attr("checked", true) : jQuery(status_ativo).attr("checked", false);
    
                //Select Cargo
                var optionCargo  = "<option value='0'> Selecione </option>";
                for(var oc = 0; oc < Retorno.ListaCargos.length; oc++)
                {
                    var selecionado = "";
                    (cod_cargo == Retorno.ListaCargos[oc].cod_cargo) ? selecionado = "selected" : selecionado = "";
    
                    optionCargo += "<option value="+Retorno.ListaCargos[oc].cod_cargo+" "+selecionado+">";
                    optionCargo += Retorno.ListaCargos[oc].tipo_cargo+"</option>";
                }
                jQuery("#cargo").html(optionCargo);
    
                //Select Permissao
                var optionPermissao  = "<option value='0'> Selecione </option>";
                for(var op = 0; op < Retorno.ListaPermissoes.length; op++)
                {
                    var selecionado = "";
                    (cod_permissao == Retorno.ListaPermissoes[op].cod_permissao) ? selecionado = "selected" : selecionado = "";
    
                    optionPermissao += "<option value="+Retorno.ListaPermissoes[op].cod_permissao+" "+selecionado+">";
                    optionPermissao += Retorno.ListaPermissoes[op].tipo_permissao+"</option>";
                }
                jQuery("#permissao").html(optionPermissao);
            }
            else
            {
                Notificar(Retorno.erro);
            }
        });
    }
    else
    {
        var Tipo = Array();
        Tipo.push('permissoes', 'cargos');
        carregarCampos(Tipo);
    }
}

function insertColaborador()
{
    var cod_colaborador     = jQuery.trim(jQuery("#cod_colaborador").val()); 
    var nome_colaborador    = jQuery.trim(jQuery("#nome_colaborador").val());
    var email               = jQuery.trim(jQuery("#email").val()); 
    var contato             = jQuery.trim(jQuery("#contato").val());
    var funcional           = jQuery.trim(jQuery("#funcional").val());

    //Dados Acesso
    var cod_acesso          = jQuery.trim(jQuery("#cod_acesso").val());
    var login               = jQuery.trim(jQuery("#login").val());
    var senha               = jQuery.trim(jQuery("#senha").val());
    var conf_senha          = jQuery.trim(jQuery("#conf_senha").val());
    var status_ativo        = (jQuery("#status_ativo").is(":checked")) ? status_ativo = 'S' : status_ativo = 'N';

    if(senha != conf_senha)
    {
        Notificar("As senhas não conferem. Favor verificar.");
        jQuery("#senha").focus();
        return false;
    }

    var Acesso = {
        cod_acesso      : cod_acesso,
        cod_colaborador : cod_colaborador,
        login           : login,
        senha           : senha,
        conf_senha      : conf_senha,
        status_ativo    : status_ativo
    };

    //Permissoes
    var cod_permissao       = jQuery.trim(jQuery("#permissao").val());

    //Cargos
    var cod_cargo           = jQuery.trim(jQuery("#cargo").val());

    var dados = {
        cod_colaborador     : cod_colaborador,
        nome_colaborador    : nome_colaborador,
        email               : email,
        contato             : contato,
        funcional           : funcional,
        cod_permissao       : cod_permissao,
        cod_cargo           : cod_cargo,
        Acesso              : Acesso
    }

    var metodo = 'insert';
    if(jQuery("#alteracao").val() == 'S')
    {
        metodo = 'update';
    }
    console.log(JSON.stringify(dados));
    consultaWS('colaboradores', metodo, dados, function(Retorno){
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

function inativarReativarColaborador(cod, ativar_inativar)
{
    var status_ativo = (ativar_inativar) ? status_ativo = 'S' : status_ativo = 'N';

    var dados = {
        cod_colaborador : cod,
        status_ativo : status_ativo    
    }

    consultaWS('acessos', 'inativarReativar', dados, function(Retorno){
        if(Retorno.status)
        {
            Notificar(Retorno.msg);

            setTimeout(function(){
                consultaWS('colaboradores', 'select', null, listaColaboradores);
            },1000);
        }
        else
        {
            Notificar(Retorno.erro);
        }
    });
}


jQuery(document).ready(function(){
    jQuery("#login").prop("disabled", true);

    jQuery("#funcional").change(function(){ 
        var funcional = jQuery("#funcional").val();

        jQuery("#login").val(funcional);
    });
});