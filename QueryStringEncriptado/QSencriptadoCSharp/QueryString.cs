using System.Collections.Specialized;
using System.Collections;
using System.Web;
//using System.Web.HttpRequest;
using System;

namespace QSencriptadoCSharp
{
    public class QueryString : NameValueCollection
    {

        private string _document;

        public string Document
        {
            get
            {
                return _document;
            }
        }

      public  QueryString()
        {
        }

        // '' <summary>
        // '' Recibe del request.querystring los valores
        // '' </summary>
        // '' <param name="clone">QueryString de la pagina actual</param>
        // '' <remarks></remarks>
      public QueryString(NameValueCollection clone) :
            base(clone)
        {
        }

        // '' <summary>
        // '' permite convertir un string en un NameValueCollection con el QueryString
        // '' </summary>
        // '' <param name="vConversion">String que concatena todo el queryString</param>
        // '' <remarks></remarks>
      public  QueryString(string vConversion) :
            base(FromUrl(vConversion))
        {
        }

        public static QueryString FromCurrent()
        {
            return FromUrl(HttpContext.Current.Request.Url.AbsoluteUri);
        }

        public static QueryString FromUrl(string url)
        {
            string[] parts = url.Split("?".ToCharArray());
            QueryString qs = new QueryString();
            qs._document = parts[0];
            if ((parts.Length == 1))
            {
                return qs;
            }
            string[] keys = parts[1].Split("&".ToCharArray());
            foreach (string key in keys)
            {
                string[] part = key.Split("=".ToCharArray());
                if ((part.Length == 1))
                {
                    qs.Add(part[0], "");
                }
                else
                {
                    qs.Add(part[0], part[1]);
                }
            }
            return qs;
        }

        public void Add(string name, string value)
        {
            if (!(this[name] == null))
            {
                this[name] = value;
            }
            else
            {
                base.Add(name, value);
            }
        }

        public string ToString()
        {
            string[] parts = new string[this.Count];
            string[] keys = this.AllKeys;
            int i = 0;
            while ((i < keys.Length))
            {
                parts[i] = keys[i] + "=" + HttpContext.Current.Server.UrlEncode(this[keys[i]]);
                // Devuelve el menor de dos enteros de 16 bits con signo. 
                // Proporciona operaciones at�micas para las variables compartidas por varios subprocesos. 
                // http://msdn.microsoft.com/es-ar/library/system.threading.interlocked.aspx
                System.Math.Min(System.Threading.Interlocked.Increment(ref i), (i - 1));
            }
            string url = string.Join("&", parts);
            if (((!(url == null)
                        || !(url == String.Empty))
                        && !url.StartsWith("?")))
            {
                url = ("?" + url);
            }
            // If False Then
            //     url = Me._document + url
            // End If
            return url;
        }
    }
}
