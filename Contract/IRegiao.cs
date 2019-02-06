using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DTO;

namespace Interfaces
{
    public interface IRegiao
    {

        List<RegiaoDTO> Consultar(Int64 IdRegiao);
        void Incluir(Int64 IdEstado, string Regiao, out string Mensagem);
        void AtivarDesativar(Int64 IdRegiao, bool Ativo);
        void Editar(Int64 IdRegiao, int IdEstado, string DescricaoRegiao, out string Mensagem);

    }
}
