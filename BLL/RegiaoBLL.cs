using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DTO;
using DAL;
using Interfaces;

namespace BLL
{
    public class RegiaoBLL : IRegiao
    {
        public List<RegiaoDTO> Consultar(Int64 IdRegiao)
        {
            using (RegiaoDAL DAL = new RegiaoDAL(false))
            {
                return DAL.Consultar(IdRegiao);
            }
        }

        public void Incluir(Int64 IdEstado, string Regiao, out string Mensagem)
        {
            using (RegiaoDAL DAL = new RegiaoDAL(true))
            {
                DAL.Incluir(IdEstado, Regiao, out Mensagem);
            }
        }

        public void Editar(Int64 IdRegiao, int IdEstado, string DescricaoRegiao, out string Mensagem)
        {
            using (RegiaoDAL DAL = new RegiaoDAL(true))
            {
                DAL.Editar(IdRegiao, IdEstado, DescricaoRegiao, out Mensagem);
            }
        }

        public void AtivarDesativar(Int64 IdRegiao, bool Ativo)
        {
            using (RegiaoDAL DAL = new RegiaoDAL(true))
            {
                DAL.AtivarDesativar(IdRegiao, Ativo);
            }
        }
    }
}
