using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DTO
{
    public class FornecedorDTO
    {

        #region Privado

        private Int64 _Id;
        private string _Cnpj;
        private string _Nome;
        private List<RegiaoDTO> _LstRegiao;

        #endregion

        #region Publico

        public Int64 Id
        {
            get
            {
                return _Id;
            }
            set
            {
                _Id = value;
            }
        }
        public string Cnpj
        {
            get
            {
                return _Cnpj;
            }
            set
            {
                _Cnpj = value;
            }
        }
        public string Nome
        {
            get { return _Nome; }
            set { _Nome = value; }
        }
        public List<RegiaoDTO> LstRegiao
        {
            get
            {
                if (_LstRegiao == null)
                    _LstRegiao = new List<RegiaoDTO>();

                return _LstRegiao;
            }
            set
            {
                _LstRegiao = value;
            }
        }

        #endregion

    }
}
