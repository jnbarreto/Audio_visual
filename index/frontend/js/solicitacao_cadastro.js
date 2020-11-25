function listaSolicitacoes(Retorno)
{
    console.log(Retorno);
    if(Retorno.status)
    {   
        var lista  = '<table id="table_id" class="table table-striped table-bordered">';
            lista += '  <thead>';
            lista += '      <tr>';
            lista += '          <th>Cod</th>';
            lista += '          <th>Nome</th>';
            lista += '          <th>E-mail</th>';
            lista += '          <th>Contato</th>';
            lista += '          <th>RF</th>';
            lista += '          <th> </th>';
            lista += '      </tr>';
            lista += '  </thead>';
            lista += '  <tbody>';

            for(var c = 0; c < Retorno.ListaSolicCadastro.length; c++)
            {
                var cod = Retorno.ListaSolicCadastro[c].cod_solic;

                lista += '<tr id="rowid'+cod+'">';
                lista += '  <td> ' + Retorno.ListaSolicCadastro[c].cod_solic + ' </td>';
                lista += '  <td> ' + Retorno.ListaSolicCadastro[c].nome.toUpperCase() + ' </td>';
                lista += '  <td> ' + Retorno.ListaSolicCadastro[c].email.toUpperCase() + ' </td>';
                lista += '  <td> ' + Retorno.ListaSolicCadastro[c].contato + ' </td>';
                lista += '  <td> ' + Retorno.ListaSolicCadastro[c].funcional.toUpperCase() + ' </td>';
                
                // Botao
                lista += '  <td width="120px">'  
                lista += '      <div class="btn-group" role="group">';
                lista += '          <button id="btnGroupDrop1" type="button" class="btn btn-success dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">';
                lista += '              Cadastrar';
                lista += '          </button>';
                lista += '          <div class="dropdown-menu" aria-labelledby="btnGroupDrop1">';
                lista += '              <a class="dropdown-item" onclick="window.location.href=\'/frontend/pages/usuarios/form.html?id='+cod+'&solicadastro='+true+' \'">Cadastrar</a>';
                //lista += '              <a class="dropdown-item" onclick="confirmaInativacao(\''+title+'\', \''+msg+'\', '+cod+', '+ativar_inativar+', inativarReativarColaborador)">'+status_ativo+'</a>';
                lista += '          </div>';
                lista += '      </div>';
                lista += '  </td>';
                // Botao

                lista += '</tr>';
            }

            lista += '  </tbody>';
            lista += '</table>';

            jQuery("#ListaSolicitacoes").html(lista);
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

function getSolicCadasro(cod)
{

    var dados = {
        cod_solic : cod
    }

    consultaWS('solicitacaocadastro', 'get', dados, function(Retorno){
        console.log(Retorno);
    
        if(Retorno.status)
        {   
            var c = 0;

            //Dados Colaborador
            var nome_colaborador    = Retorno.ListaSolicCadastro[c].nome;
            var email               = Retorno.ListaSolicCadastro[c].email;
            var funcional           = Retorno.ListaSolicCadastro[c].funcional;
            var contato             = Retorno.ListaSolicCadastro[c].contato;

            //Inserção de dados em campos input dados colaborador
            jQuery("#nome_colaborador").val(nome_colaborador).prop("disabled", true);
            jQuery("#contato").val(contato).prop("disabled", true);
            jQuery("#email").val(email).prop("disabled", true);
            jQuery("#funcional").val(funcional).prop("disabled", true);

            //Inserção de dados em campos input acesso
            jQuery("#login").val(funcional).prop("disabled", true);

        }
        else
        {
            Notificar(Retorno.erro);
        }
    });

    setTimeout(function(){
        var Tipo = Array();
        Tipo.push('permissoes', 'cargos');
        carregarCampos(Tipo);
    },2000);
}

function insertSolicitacao()
{
    var funcional   = jQuery("#solic_funcional").val();
    var nome        = jQuery("#solic_nome").val();
    var email       = jQuery("#solic_email").val();
    var contato     = jQuery("#solic_contato").val();

    var dados = {
        funcional : funcional,
        nome : nome,
        email : email,
        contato : contato
    }

    consultaWS('solicitacaocadastro', 'insert', dados, function(Retorno)
    {
        if(Retorno.status)
        {
            Notificar(Retorno.msg);

            setTimeout(function(){
                //IrPara('/frontend/pages/page-login.html');
            }, 3000);
        }
        else
        {
            Notificar(Retorno.erro);
        }
    });
}

function msgSolicCadastro(msg, tipo_status)
{
    switch(tipo_status)
    {
        case 'N':
            jQuery.confirm({
                title: "Alerta Audio Visual",
                content: msg,
                closeIcon: false,
                type: 'orange',
                typeAnimated: true,
                draggable : false,
                
                buttons: {
                    formSubmit: {
                        text: 'OK',
                        btnClass: 'btn-primary',
                        action: function () {
                            
                        }
                    },
                },
        
                onContentReady: function () {
                    // bind to events
                    var jc = this;
                    this.$content.find('form').on('submit', function (e) {
                        // if the user submits the form by pressing enter in the field.
                        e.preventDefault();
                        jc.$$formSubmit.trigger('click'); // reference the button and click it
                    });
                },
                onOpenBefore: function () {},
                onOpen: function () {},
                onClose: function () {},
                onDestroy: function () {},
                onAction: function () {}
            });
        break;

        case 'C':
            jQuery.confirm({
                title: "Alerta Audio Visual",
                content: msg,
                closeIcon: false,
                type: 'orange',
                typeAnimated: true,
                draggable : false,

                buttons: {
                    formSubmit: {
                        text: 'Alterar Senha',
                        btnClass: 'btn-primary',
                        action: function () {
                            
                        }
                    },
                    acessar: function () {
                        IrPara('/frontend/pages/page-login.html');
                    }
                },
        
                onContentReady: function () {
                    // bind to events
                    var jc = this;
                    this.$content.find('form').on('submit', function (e) {
                        // if the user submits the form by pressing enter in the field.
                        e.preventDefault();
                        jc.$$formSubmit.trigger('click'); // reference the button and click it
                    });
                },
                onOpenBefore: function () {},
                onOpen: function () {},
                onClose: function () {},
                onDestroy: function () {},
                onAction: function () {}
            });
        break;
    }
}



jQuery(document).ready(function(){

    jQuery("#solic_funcional").focus();

    jQuery('#solic_nome').attr("disabled", "true");
    jQuery('#solic_email').attr("disabled", "true");
    jQuery('#solic_contato').attr("disabled", "true");
    jQuery('#solicitar_cadastro').attr("disabled", "true");

    jQuery("#solic_funcional").change(function()
    {

        var dados = {
            funcional : jQuery("#solic_funcional").val()
        }

        if(jQuery.trim(jQuery("#solic_funcional").val()).length == 0)
        {
            jQuery('#solic_nome').attr("disabled", "true");
            jQuery('#solic_email').attr("disabled", "true");
            jQuery('#solic_contato').attr("disabled", "true");
            jQuery('#solicitar_cadastro').attr("disabled", "true");

            jQuery("#solic_funcional").focus();

            return false;
        }

        consultaWS('solicitacaocadastro', 'getRF', dados, function(Retorno){
            if(!Retorno.status)
            {
                switch(Retorno.ListaSolicCadastro[0].status_cadastros)
                {
                    case 'N':
                        msgSolicCadastro(Retorno.erro, 'N');
                    break;

                    case 'C':
                        msgSolicCadastro(Retorno.erro, 'C');
                    break;
                }
            }
            else
            {
                jQuery('#solic_nome').removeAttr("disabled");
                jQuery('#solic_email').removeAttr("disabled");
                jQuery('#solic_contato').removeAttr("disabled");
                jQuery('#solicitar_cadastro').removeAttr("disabled");

                jQuery("#solic_nome").focus();
            }
        });
    });

    jQuery("#solicitar_cadastro").click(function(){
        insertSolicitacao();
    });
});