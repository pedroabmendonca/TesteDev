using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Contract;
using DTO;
using DAL;
using System.Data;

namespace BLL
{
    public class FornecedorBLL : IFornecedor
    {
        public List<FornecedorDTO> Consultar(Int64 Id)
        {
            using (FornecedorDAL DAL = new FornecedorDAL(false))
            {
                return DAL.Consultar(Id);
            }
        }

        public FornecedorDTO ConsultarFornecedorRegiao(Int64 IdFornecedor)
        {
            using (FornecedorDAL DAL = new FornecedorDAL(false))
            {
                return DAL.ConsultarFornecedorRegiao(IdFornecedor);
            }
        }

        public void Editar(Int64 IdFornecedor, DataTable FornecedorRegiao)
        {
            using (FornecedorDAL DAL = new FornecedorDAL(false))
            {
                DAL.Editar(IdFornecedor, FornecedorRegiao);
            }
        }
    }
}
