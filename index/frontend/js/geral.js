function consultaWS(funcao, metodo , dados, callBackFunction, showLoading = true)
{
    var jsonData = JSON.stringify(jQuery.extend({} , dados));

    console.log(jsonData);

    var url = "/backend/"+funcao+"/"+metodo;
    var _retorno = {status: false, msg:"", erro : "Não foi possivel consultar o webservice. Tente novamente mais tarde!"};

    jQuery.ajax({
        type: "POST",
        dataType: "json",
        data: jsonData,
        url: url,
        async : true,
        contentType: "application/json; charset=utf-8",
        beforeSend : function()
        {
            if (showLoading)
            {
                CustomLoading();
            }
        }
    })
    .done(function(Retorno)
    {
        _retorno = Retorno;
    })
    
    .fail(function(Retorno)
    {

    })

    .always (function(Retorno)
    {
        if(showLoading)
        {
            CustomUnloading();
        }
        if(callBackFunction != null)
        {
            callBackFunction(Retorno);
        }

    });
    return _retorno;
}

function CustomLoading(){
	jQuery('body').prepend('<div class="CustomLoading">'+
	                 '</div>'+
					 '<div class="CustomLoading2">'+
					 '	<div class="CustomLoading22">'+
					 '   	<div class="ball"></div>'+
					 '   	<div class="ball1"></div>'+
					 '   	<div class="CustomTexto">Aguarde um instante...<br>Estamos processando sua solicitação</div>'+
					 '	</div>'+
					 '</div>');
}

function CustomUnloading(){
	jQuery('.ball').remove();
	jQuery('.ball1').remove();
	jQuery('.CustomTexto').remove();
	jQuery('.CustomLoading2').remove();
	jQuery('.CustomLoading').remove();
}

function leftPanel()
{
    var m1  = '<li class="active" id="li_dashboard">';
        m1 += ' <a href="/frontend/pages/dashboard/dashboard.html"> <i class="menu-icon fa fa-dashboard"></i>Dashboard </a>';
        m1 += '</li>';

    var m2  = '<h3 class="menu-title" id="h3_colaborador">Colaborador</h3>';
        m2 += ' <li class="menu-item-has-children dropdown" id="li_colaborador">';
        m2 += '     <a href="#" class="dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"> <i class="menu-icon fa fa-users"></i>Usuarios</a>';
        m2 += '     <ul class="sub-menu children dropdown-menu">';
        m2 += '        <li><i class="fa fa-users"></i><a href="/frontend/pages/usuarios/">Colaboradores</a></li>';
        m2 += '        <li><i class="fa fa-bullhorn"></i><a href="/frontend/pages/usuarios/solic_cadastro.html">Solic. Cadastro</a></li>';
        m2 += '        <li><i class="fa fa-user novo_colaborador"></i><a href="/frontend/pages/usuarios/form.html">Novo Colaborador</a></li>';
        m2 += '     </ul>';
        m2 += ' </li>';

    var m3  = '<h3 class="menu-title" id="h3_acessos">Acessos</h3>';
        m3 += ' <li class="menu-item-has-children dropdown" id="li_acessos">';
        m3 += '     <a href="#" class="dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"> <i class="menu-icon fa fa-key"></i>Permissões</a>';
        m3 += '     <ul class="sub-menu children dropdown-menu">';
        m3 += '        <li><i class="fa fa-key"></i><a href="/frontend/pages/permissoes/">Permissões Cad.</a></li>';
        m3 += '        <li><i class="fa fa-key"></i><a href="/frontend/pages/permissoes/form.html">Nova Permissão</a></li>';
        m3 += '     </ul>';
        m3 += ' </li>';

    var m4  = '<h3 class="menu-title" id="h3_responsabilidade">Responsabilidades</h3>';
        m4 += ' <li class="menu-item-has-children dropdown" id="li_responsabilidade">';
        m4 += '     <a href="#" class="dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"> <i class="menu-icon fa fa-briefcase"></i>Responsabilidade</a>';
        m4 += '     <ul class="sub-menu children dropdown-menu">';
        m4 += '        <li><i class="fa fa-briefcase"></i><a href="/frontend/pages/cargos/">Cargos Cadastrados</a></li>';
        m4 += '        <li><i class="fa fa-briefcase"></i><a href="/frontend/pages/cargos/form.html">Novo Cargo</a></li>';
        m4 += '     </ul>';
        m4 += ' </li>';

    var m5  = '<h3 class="menu-title" id="h3_agendamento">Agendamento</h3>';
        m5 += ' <li class="menu-item-has-children dropdown" id="li_agendamento">';
        m5 += '     <a href="#" class="dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"> <i class="menu-icon fa fa-ellipsis-h"></i>Status Agend.</a>';
        m5 += '     <ul class="sub-menu children dropdown-menu">';
        m5 += '        <li><i class="fa fa-ellipsis-h"></i><a href="/frontend/pages/status_agendamento/">Status Cadastrados</a></li>';
        m5 += '        <li><i class="fa fa-ellipsis-h"></i><a href="/frontend/pages/status_agendamento/form.html">Novo Status</a></li>';
        m5 += '     </ul>';
        m5 += ' </li>';
    
    var m6 =  ' <li class="menu-item-has-children dropdown" id="li_reserva">';
        m6 += '     <a href="#" class="dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"> <i class="menu-icon fa fa-calendar-o"></i>Reservar</a>';
        m6 += '     <ul class="sub-menu children dropdown-menu">';
        m6 += '        <li><i class="fa fa-calendar-o"></i><a href="/frontend/pages/agendamento/">Reservar Equip.</a></li>';
        m6 += '     </ul>';
        m6 += ' </li>';

    var m7  = '<h3 class="menu-title" id="h3_dispositivos">Dispositivo Eletrônico</h3>';
        m7 += ' <li class="menu-item-has-children dropdown"  id="li_dispositivos">';
        m7 += '     <a href="#" class="dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"> <i class="menu-icon fa fa-desktop"></i>Equipamentos</a>';
        m7 += '     <ul class="sub-menu children dropdown-menu">';
        m7 += '        <li><i class="fa fa-keyboard-o"></i><a href="/frontend/pages/equipamentos/">Equipamentos</a></li>';
        m7 += '        <li><i class="fa fa-keyboard-o"></i><a href="/frontend/pages/equipamentos/form.html">Novo Equipamento</a></li>';
        m7 += '     </ul>';
        m7 += ' </li>';

    var m8  = '<h3 class="menu-title" id="h3_relatorio_mensal">Relatório Mensal</h3>';
        m8 += ' <li class="menu-item-has-children dropdown"  id="li_relatorio_mensal">';
        m8 += '     <a href="#" class="dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"> <i class="menu-icon fa fa-archive"></i>Relatório</a>';
        m8 += '     <ul class="sub-menu children dropdown-menu">';
        m8 += '        <li><i class="fa fa-folder"></i><a href="/frontend/pages/relatorio/">Gerar</a></li>';
        m8 += '     </ul>';
        m8 += ' </li>';
    
    var leftPanel  = '<ul class="nav navbar-nav">';
        leftPanel +=    m1 + m2 + m3 + m4 + m5 + m6 + m7 + m8;
        leftPanel += '</ul>';

    jQuery("#main-menu").html(leftPanel);

    menu_verifica_permissao();
}

function menu_verifica_permissao()
{
        var session         = getSessionStorage("AcessoColaborador");
        var cod_permissao   = session.permissao.cod_permissao;

        var listaIdMenuProf = [
            {id : "#li_dashboard"        , ativo : false  },
            {id : "#h3_colaborador"      , ativo : false  },
            {id : "#li_colaborador"      , ativo : false  },
            {id : "#h3_acessos"          , ativo : false  },
            {id : "#li_acessos"          , ativo : false  },
            {id : "#h3_responsabilidade" , ativo : false  },
            {id : "#li_responsabilidade" , ativo : false  },
            {id : "#h3_agendamento"      , ativo : true   },
            {id : "#li_agendamento"      , ativo : false  },
            {id : "#li_reserva"          , ativo : true   },
            {id : "#h3_dispositivos"     , ativo : false  },
            {id : "#li_dispositivos"     , ativo : false  }];

        for(var i = 0; i < listaIdMenuProf.length; i++)
        {
            if(cod_permissao == 4 && listaIdMenuProf[i].ativo == false)
            {
                jQuery(listaIdMenuProf[i].id).hide();
            }
        }
}

function get_permissao()
{
    var session         = getSessionStorage("AcessoColaborador");
    var cod_permissao   = session.permissao.cod_permissao;

    return cod_permissao
}

//Captura dados da URL
function getUrlParametro() 
{
    var prmstr = window.location.search.substr(1);
    return prmstr != null && prmstr != "" ? transformeArray(prmstr) : {};
}

//Trasforma dados em array
function transformeArray( prmstr ) 
{
    var params = {};
    var prmarr = prmstr.split("&");
    for ( var i = 0; i < prmarr.length; i++) 
    {
        var tmparr = prmarr[i].split("=");
        params[tmparr[0]] = tmparr[1];
    }
    return params;
}

function Notificar(msg = "")
{
    jQuery.dialog({
        title: 'Alerta Audio Visual',
        content: msg,
        type: 'orange',
        typeAnimated: true,
    });
}

function IrPara(page)
{
    window.location.replace(page);
}

function confirmaInativacao(title, msg, cod, ativar_inativar, callback)
{
    jQuery.confirm({
        title: title,
        content: msg,
        closeIcon: false,
        type: 'orange',
        typeAnimated: true,
        draggable : false,

        buttons: {
            formSubmit: {
                text: 'Confirmar',
                btnClass: 'btn-primary',
                action: function () {
                    callback(cod, ativar_inativar);
                }
            },
            cancelar: function () {
                //close
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
}

function setVal(id1, id2, val1, val2)
{
    jQuery("#"+id1).val(val1);
    jQuery("#"+id2).val(val2);
}

function carregarCampos(Tipo = null)
{
    console.log(Tipo);
    consultaWS('carregalistas', 'montarlista', null, function(Retorno){
        console.log(Retorno);

        if(Retorno.status)
        {  
            if(Tipo.length > 0)
            {
                for (let t = 0; t < Tipo.length; t++) 
                {
                    switch(Tipo[t])
                    {
                        case 'permissoes':
                            //Select Permissao
                            var optionPermissao  = "<option value='0'> Selecione </option>";
                            for(var op = 0; op < Retorno.ListaPermissoes.length; op++)
                            {
                                optionPermissao += "<option value="+Retorno.ListaPermissoes[op].cod_permissao+">";
                                optionPermissao += Retorno.ListaPermissoes[op].tipo_permissao+"</option>";
                            }
                            jQuery("#permissao").html(optionPermissao);
                        break;

                        case 'cargos':
                            //Select Cargo
                            var optionCargo  = "<option value='0'> Selecione </option>";
                            for(var oc = 0; oc < Retorno.ListaCargos.length; oc++)
                            {
                                optionCargo += "<option value="+Retorno.ListaCargos[oc].cod_cargo+">";
                                optionCargo += Retorno.ListaCargos[oc].tipo_cargo+"</option>";
                            }
                            jQuery("#cargo").html(optionCargo);
                        break;
                    }
                }
            }
        }
    },false);
}

function validaAcesso(doc)
{
	if (sessionStorage.getItem("SessionAV") == undefined)
    {
        doc.document.body.style.display = "none";
        sairDoSistema();
		IrPara('/frontend/pages/page-login.html');
	} 
    else 
    {
		doc.document.body.style.display = "block";
	}
}

function sairDoSistema()
{

    var login = getLocalStorage('login');
    if(login.auto_login)
    {
        desativaAutoLogin();
    }

    localStorage.clear();
    sessionStorage.clear();

    IrPara('/frontend/pages/page-login.html');
    
}

function limpar_div(listIDS = null)
{
    if(listIDS.length > 0)
    {
        for(let i = 0; i < listIDS.length; i++)
        {
            jQuery("#"+listIDS[i].id).empty();
            jQuery("#"+listIDS[i].id).load();
        }
    }
}

function formate_date(d) 
{

    var data            = new Date(d);
    var dia             = data.getDate();
    var mes             = data.getMonth() + 1;
    var ano             = data.getFullYear();
    var data_agendado   = dia + "/" + mes + "/" + ano;

    return data_agendado;
}

jQuery(document).ready(function(){
    jQuery("#logout").click(function(){
        sairDoSistema();
    });
});