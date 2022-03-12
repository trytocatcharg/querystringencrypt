using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Page1 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        //Response.Redirect("Page2.aspx?p1=".ToString() + txtValor1.Text.ToString() + "&p2=" + txtValor2.Text.ToString());


        //1 primero creo un objeto Clave/Valor de QueryString 
        QSencriptadoCSharp.QueryString qs = new QSencriptadoCSharp.QueryString();
        //QSencriptado.QueryString qs = new QSencriptado.QueryString();

        //2 voy a agregando los valores que deseo
        qs.Add("p1", txtValor1.Text);
        qs.Add("p2", txtValor2.Text);


        //Encripto el objeto y UTILIZO LA PROPIEDAD ToString() para devolver una cadena encriptada
        Response.Redirect("Page2.aspx" + QSencriptadoCSharp.Encryption.EncryptQueryString(qs).ToString());
        //Response.Redirect("Page2.aspx" + QSencriptado.Encryption.EncryptQueryString(qs).ToString());

    }
}