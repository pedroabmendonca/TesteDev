using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DTO
{
    public class RegiaoDTO
    {

        #region Privado

        private Int64 _Id;
        private EstadoDTO _Estado;
        private string _Descricao;
        private bool _Ativo;

        #endregion

        #region Publico

        public Int64 Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        public EstadoDTO Estado
        {
            get
            {
                if (_Estado == null)
                    _Estado = new EstadoDTO();

                return _Estado;
            }
            set { _Estado = value; }
        }

        public string Descricao
        {
            get { return _Descricao; }
            set { _Descricao = value; }
        }

        public bool Ativo
        {
            get { return _Ativo; }
            set { _Ativo = value; }
        }

        #endregion

    }
}
