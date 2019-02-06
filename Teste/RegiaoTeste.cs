using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BLL;
using DTO;

namespace Teste
{
    [TestClass]
    public class RegiaoTeste
    {
        [TestMethod]
        public void Consulta()
        {
            //Parametros
            Int64 IdFornecedor = Int64.MinValue;

            //Negocio
            RegiaoBLL negocio = new RegiaoBLL();
            List<RegiaoDTO> LstRegiao = negocio.Consultar(IdFornecedor);

            //Assert
            Assert.AreNotEqual(null, LstRegiao);
        }

        [TestMethod]
        public void Consulta2()
        {
            //Parametros
            Int64 IdFornecedor = Int64.MinValue;

            //Negocio
            RegiaoBLL negocio = new RegiaoBLL();
            negocio.Consultar(IdFornecedor);

            //Assert

        }

        [TestMethod]
        public void Incluir()
        {
        }

        [TestMethod]
        public void Editar()
        {
        }

        [TestMethod]
        public void AtivarDesativar()
        {
        }
    }
}
