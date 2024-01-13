global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Mvc;
global using PharmacyWebAPI.DataAccess.Repository.IRepository;
global using PharmacyWebAPI.Models;
using AutoMapper;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using NuGet.Protocol;
using System.Text;

namespace PharmacyWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public IActionResult Index()
        {
            /*using var client = new HttpClient();*/

            /*            var result = await client.GetStringAsync("https://localhost:44332/api/category/getall");
            */
            var result = Drow("EzDrug APIS");
            return Ok(result);
        }

        [HttpPost]
        [Route("redirect")]
        public IActionResult tryRedirect()
        {
            /*using var client = new HttpClient();*/

            /*            var result = await client.GetStringAsync("https://localhost:44332/api/category/getall");
            */
            return Redirect("google.com");
        }

        /*
         * using System.Text;
        using Newtonsoft.Json;

        var person = new Person("John Doe", "gardener");

        var json = JsonConvert.SerializeObject(person);
        var data = new StringContent(json, Encoding.UTF8, "application/json");

        var url = "https://httpbin.org/post";
        using var client = new HttpClient();

        var response = await client.PostAsync(url, data);

        var result = await response.Content.ReadAsStringAsync();
        Console.WriteLine(result);

        record Person(string Name, string Occupation);

         */

        private static string Drow(string text)
        {
            Dictionary<int, string[]> Letters = new();
            int counter = 0;
            foreach (var letter in text)
            {
                if (char.IsDigit(letter))
                    throw new Exception("Drow Letters only , for now ");
                switch (char.ToLower(letter))
                {
                    case 'a':
                        string[] a = new string[5];
                        a[0] = "    ##    ";
                        a[1] = "  #    #  ";
                        a[2] = "  ######  ";
                        a[3] = "  #    #  ";
                        a[4] = "  #    #  ";
                        Letters.Add(counter, a);
                        break;

                    case 'b':
                        string[] b = new string[5];
                        b[0] = "  ######  ";
                        b[1] = "  #    #  ";
                        b[2] = "  #####   ";
                        b[3] = "  #    #  ";
                        b[4] = "  ######  ";
                        Letters.Add(counter, b);
                        break;

                    case 'c':
                        string[] c = new string[5];
                        c[0] = "  ######  ";
                        c[1] = "  #       ";
                        c[2] = "  #       ";
                        c[3] = "  #       ";
                        c[4] = "  ######  ";
                        Letters.Add(counter, c);
                        break;

                    case 'd':
                        string[] d = new string[5];
                        d[0] = "  #####   ";
                        d[1] = "  #    #  ";
                        d[2] = "  #    #  ";
                        d[3] = "  #    #  ";
                        d[4] = "  #####   ";
                        Letters.Add(counter, d);
                        break;

                    case 'e':
                        string[] e = new string[5];
                        e[0] = "  ######  ";
                        e[1] = "  #       ";
                        e[2] = "  #####   ";
                        e[3] = "  #       ";
                        e[4] = "  ######  ";
                        Letters.Add(counter, e);
                        break;

                    case 'f':
                        string[] f = new string[5];
                        f[0] = "  ######  ";
                        f[1] = "  #       ";
                        f[2] = "  #####   ";
                        f[3] = "  #       ";
                        f[4] = "  #       ";
                        Letters.Add(counter, f);
                        break;

                    case 'g':
                        string[] g = new string[5];
                        g[0] = "  ######  ";
                        g[1] = "  #       ";
                        g[2] = "  #  #### ";
                        g[3] = "  #    #  ";
                        g[4] = "  ######  ";
                        Letters.Add(counter, g);
                        break;

                    case 'h':
                        string[] h = new string[5];
                        h[0] = "  #    #  ";
                        h[1] = "  #    #  ";
                        h[2] = "  ######  ";
                        h[3] = "  #    #  ";
                        h[4] = "  #    #  ";
                        Letters.Add(counter, h);
                        break;

                    case 'i':
                        string[] i = new string[5];
                        i[0] = "  ######  ";
                        i[1] = "    ##    ";
                        i[2] = "    ##    ";
                        i[3] = "    ##    ";
                        i[4] = "  ######  ";
                        Letters.Add(counter, i);
                        break;

                    case 'j':
                        string[] j = new string[5];
                        j[0] = "  ######  ";
                        j[1] = "    ##    ";
                        j[2] = "    ##    ";
                        j[3] = "  # ##    ";
                        j[4] = "  ####    ";
                        Letters.Add(counter, j);
                        break;

                    case 'k':
                        string[] k = new string[5];
                        k[0] = "  #  #    ";
                        k[1] = "  # #     ";
                        k[2] = "  ##      ";
                        k[3] = "  #  #    ";
                        k[4] = "  #  #    ";
                        Letters.Add(counter, k);
                        break;

                    case 'l':
                        string[] l = new string[5];
                        l[0] = "  #       ";
                        l[1] = "  #       ";
                        l[2] = "  #       ";
                        l[3] = "  #       ";
                        l[4] = "  ######  ";
                        Letters.Add(counter, l);
                        break;

                    case 'm':
                        string[] m = new string[5];
                        m[0] = "  #    #  ";
                        m[1] = "  ##  ##  ";
                        m[2] = "  # ## #  ";
                        m[3] = "  #    #  ";
                        m[4] = "  #    #  ";
                        Letters.Add(counter, m);
                        break;

                    case 'n':
                        string[] n = new string[5];
                        n[0] = "  #    #  ";
                        n[1] = "  ##   #  ";
                        n[2] = "  # #  #  ";
                        n[3] = "  #  # #  ";
                        n[4] = "  #   ##  ";
                        Letters.Add(counter, n);
                        break;

                    case 'o':
                        string[] o = new string[5];
                        o[0] = "  ######  ";
                        o[1] = "  #    #  ";
                        o[2] = "  #    #  ";
                        o[3] = "  #    #  ";
                        o[4] = "  ######  ";
                        Letters.Add(counter, o);
                        break;

                    case 'p':
                        string[] p = new string[5];
                        p[0] = "  ######  ";
                        p[1] = "  #    #  ";
                        p[2] = "  ######  ";
                        p[3] = "  #       ";
                        p[4] = "  #       ";
                        Letters.Add(counter, p);
                        break;

                    case 'q':
                        string[] q = new string[5];
                        q[0] = "  ######  ";
                        q[1] = "  #    #  ";
                        q[2] = "  #  # #  ";
                        q[3] = "  ######  ";
                        q[4] = "        # ";
                        Letters.Add(counter, q);
                        break;

                    case 'r':
                        string[] r = new string[5];
                        r[0] = "  ######  ";
                        r[1] = "  #    #  ";
                        r[2] = "  # ##    ";
                        r[3] = "  #   #   ";
                        r[4] = "  #    #  ";
                        Letters.Add(counter, r);
                        break;

                    case 's':
                        string[] s = new string[5];
                        s[0] = "  ######  ";
                        s[1] = "  #       ";
                        s[2] = "  ######  ";
                        s[3] = "       #  ";
                        s[4] = "  ######  ";
                        Letters.Add(counter, s);
                        break;

                    case 't':
                        string[] t = new string[5];
                        t[0] = "  ######  ";
                        t[1] = "    ##    ";
                        t[2] = "    ##    ";
                        t[3] = "    ##    ";
                        t[4] = "    ##    ";
                        Letters.Add(counter, t);
                        break;

                    case 'u':
                        string[] u = new string[5];
                        u[0] = "  #    #  ";
                        u[1] = "  #    #  ";
                        u[2] = "  #    #  ";
                        u[3] = "  #    #  ";
                        u[4] = "  ######  ";
                        Letters.Add(counter, u);
                        break;

                    case 'v':
                        string[] v = new string[5];
                        v[0] = "  #    #  ";
                        v[1] = "  #    #  ";
                        v[2] = "  #    #  ";
                        v[3] = "  #    #  ";
                        v[4] = "    ##    ";
                        Letters.Add(counter, v);
                        break;

                    case 'w':
                        string[] w = new string[5];
                        w[0] = "  #    #  ";
                        w[1] = "  #    #  ";
                        w[2] = "  # ## #  ";
                        w[3] = "  ##  ##  ";
                        w[4] = "  #    #  ";
                        Letters.Add(counter, w);
                        break;

                    case 'x':
                        string[] x = new string[5];
                        x[0] = "  #    #  ";
                        x[1] = "   #  #   ";
                        x[2] = "    ##    ";
                        x[3] = "   #  #   ";
                        x[4] = "  #    #  ";
                        Letters.Add(counter, x);
                        break;

                    case 'y':
                        string[] y = new string[5];
                        y[0] = "  #    #  ";
                        y[1] = "   #  #   ";
                        y[2] = "    ##    ";
                        y[3] = "    ##    ";
                        y[4] = "    ##    ";
                        Letters.Add(counter, y);
                        break;

                    case 'z':
                        string[] z = new string[5];
                        z[0] = "  ####### ";
                        z[1] = "       #  ";
                        z[2] = "     #    ";
                        z[3] = "   #      ";
                        z[4] = "  ####### ";
                        Letters.Add(counter, z);
                        break;

                    case ' ':
                        string[] space = new string[5];
                        space[0] = "   ";
                        space[1] = "   ";
                        space[2] = "   ";
                        space[3] = "   ";
                        space[4] = "   ";
                        Letters.Add(counter, space);
                        break;
                }
                counter++;
            }
            StringBuilder stringBuilder = new();

            Console.WriteLine("\n\n");
            for (int ii = 0; ii < 5; ii++)
            {
                string line = string.Empty;
                for (int jj = 0; jj < text.Length; jj++)
                    line += (Letters[jj])[ii];
                stringBuilder.AppendLine(line);
            }

            return stringBuilder.ToString();
        }
    }
}