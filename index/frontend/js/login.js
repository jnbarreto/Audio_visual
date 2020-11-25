function Login()
{
    var auto_login = jQuery("#lembre-me").is(":checked");

    var dados = {
        login : jQuery("#funcional").val(),
        senha : jQuery("#senha").val(),
        auto_login : auto_login
    }

    consultaWS('login', 'login', dados, function(Retorno)
    {
        if(Retorno.status)
        {
            var colaborador = {

                cod_colaborador : Retorno.ListaCookies[0].cod_colaborador,
                nome_colaborador: Retorno.ListaCookies[0].nome_colaborador,
                email           : Retorno.ListaCookies[0].email,
                contato         : Retorno.ListaCookies[0].contato,
                funcional       : Retorno.ListaCookies[0].funcional,

                permissao       : {
                    cod_permissao : Retorno.ListaCookies[0].cod_permissao,
                    tipo_permissao: Retorno.ListaCookies[0].tipo_permissao
                },

                cargo           : {
                    cod_cargo   : Retorno.ListaCookies[0].cod_cargo,
                    tipo_cargo  : Retorno.ListaCookies[0].tipo_cargo
                }

            }

            sessionStorage.setItem('SessionAV', 1);
            sessionStorage.setItem('AcessoColaborador', JSON.stringify(colaborador));

            if(auto_login)
            {
                ativaAutoLogin(Retorno);
            }

            var permissao = get_permissao();
            (permissao == 1) ? IrPara('/frontend/pages/dashboard/dashboard.html') : IrPara('/frontend/pages/agendamento/');

        }
        else
        {
            Notificar(Retorno.erro);
        }
    });
}

function AutoLogin()
{
    var login = getLocalStorage('login');
    var dados = {
        cod_auto_login : login.cod_auto_login
    }
    consultaWS('login', 'loginAutomatico', dados, function(Retorno){
        if(Retorno.status)
        {
            var colaborador = {

                cod_colaborador : Retorno.ListaCookies[0].cod_colaborador,
                nome_colaborador: Retorno.ListaCookies[0].nome_colaborador,
                email           : Retorno.ListaCookies[0].email,
                contato         : Retorno.ListaCookies[0].contato,

                permissao       : {
                    cod_permissao : Retorno.ListaCookies[0].cod_permissao,
                    tipo_permissao: Retorno.ListaCookies[0].tipo_permissao
                },

                cargo           : {
                    cod_cargo   : Retorno.ListaCookies[0].cod_cargo,
                    tipo_cargo  : Retorno.ListaCookies[0].tipo_cargo
                }

            }
            sessionStorage.setItem('SessionAV', 1);
            sessionStorage.setItem('AcessoColaborador', JSON.stringify(colaborador));

            IrPara('/frontend/pages/dashboard/dashboard.html');
        }
        else
        {
            Notificar(Retorno.erro);
        }
    });
}

function ativaAutoLogin(Retorno)
{

    var uid = {
        auto_login : Retorno.ListaAcessos[0].auto_login,
        cod_auto_login: Retorno.ListaAcessos[0].cod_auto_login
    }

    //login é tua chave, true é teu valor e 43200 são os minutos de vida
    localStorage.setItem('SessionUID', 1);
    setLocalStorage('login', uid, 43200);

}

function desativaAutoLogin()
{
    var login = getLocalStorage('login');
    var cod_auto_login = login.cod_auto_login;

    var dados = {
        cod_auto_login : cod_auto_login
    }
    consultaWS('login', 'desativaAutoLogin', dados, function(Retorno){
        if(Retorno.status)
        {
            console.log(Retorno.msg);
        }
        else
        {
            console.log(Retorno.erro)
        }
    },false);
}

function solicitarRecuperarSenha()
{
    var dados = {
        funcional : jQuery("#recfuncional").val(),
        email : jQuery("#recemail").val(),
    }

    consultaWS('senha', 'recuperaSenha', dados, function(Retorno){
        if(Retorno.status)
        {
            Notificar(Retorno.msg);
            document.getElementById('recfuncional').value='';
            document.getElementById('recemail').value='';
        }
        else
        {
            Notificar(Retorno.erro);
        }
    });
}

function AtualizaSenha()
{
    if(jQuery("#upsenha").val() == jQuery("#confsenha").val())
    {
        var dados = {
            login : jQuery("#uplogin").val(),
            senha : jQuery("#upsenha").val(),
            // confSenha : jQuery("confsenha").val(),
        }
        consultaWS('senha', "update", dados, function(Retorno){
            if(Retorno.status)
            {
                Notificar(Retorno.msg);
                document.getElementById('uplogin').value  ='';
                document.getElementById('upsenha').value  ='';
                document.getElementById('confsenha').value='';
            }
            else
            {
                Notificar(Retorno.erro);
            }
        });
    }else
    {
        Notificar("Senha não coincide com a anterior");
    }

}

jQuery(document).ready(function()
{
    jQuery("#funcional").focus();

    jQuery("#acesso").click(function(){
        Login();
    });

    jQuery("#funcional").keypress(function(e){
        if(e.which == 13) {
            jQuery("#acesso").click();
        }
    });

    jQuery("#senha").keypress(function(e){
        if(e.which == 13) {
            jQuery("#acesso").click();
        }
    });

    jQuery("#recuperar").click(function(){
        solicitarRecuperarSenha()
    });

    jQuery("#recfuncional").keypress(function(e){
        if(e.which == 13) {
            jQuery("#recuperar").click();
        }
    });
    jQuery("#recemail").keypress(function(e){
        if(e.which == 13) {
            jQuery("#recuperar").click();
        }
    });
    jQuery("#upconfirma").click(function(){
        AtualizaSenha()
    });

    jQuery("#uplogin").keypress(function(e){
        if(e.which == 13) {
            jQuery("#upconfirma").click();
        }
    });
    jQuery("#upsenha").keypress(function(e){
        if(e.which == 13) {
            jQuery("#upconfirma").click();
        }
    });
    jQuery("#confsenha").keypress(function(e){
        if(e.which == 13) {
            jQuery("#upconfirma").click();
        }
    });
});
