using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DTO;
using System.Data;

namespace Contract
{
    public interface IFornecedor
    {

        List<FornecedorDTO> Consultar(Int64 Id);
        void Editar(Int64 IdFornecedor, DataTable FornecedorRegiao);
        FornecedorDTO ConsultarFornecedorRegiao(Int64 IdFornecedor);

    }
}
