using System;
using System.Collections.Generic; 
using System.Text;

namespace CodeNation2020
{
    class Program
    {
        static void Main(string[] args)
        {
            //PrimeiraParte, OK! :)   
            
            DesafioJson desafio = new DesafioJson();
            desafio.GetDesafioWeb();
            string path = @"C:\Users\aline\source\repos\CodeNation2020\answer.json";
            desafio.GetInfoFileJson(path, desafio);
            desafio.SetInfoFileJson(path, desafio);
            desafio.Enviar();

        }

    }


}

