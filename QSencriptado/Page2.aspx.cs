using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Page2 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

        //Label1.Text ="El valor del parametro 1 es: " +  Request.QueryString["p1"].ToString();
        //Label2.Text ="El valor del parametro 2 es: " + Request.QueryString["p2"].ToString();

        
        
        //1- guardo el Querystring encriptado que viene desde el request en mi objeto
        QSencriptadoCSharp.QueryString qs = new QSencriptadoCSharp.QueryString(Request.QueryString);
        //QSencriptado.QueryString qs = new QSencriptado.QueryString(Request.QueryString);

        ////2- Descencripto y de esta manera obtengo un array Clave/Valor normal
        qs = QSencriptadoCSharp.Encryption.DecryptQueryString(qs);
        //qs = QSencriptado.Encryption.DecryptQueryString(qs);


        Label1.Text = "El valor del parametro 1 es: " + qs["p1"].ToString();
        Label2.Text = "El valor del parametro 2 es: " + qs["p2"].ToString();


    }
}