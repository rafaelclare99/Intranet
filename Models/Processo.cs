using System;
using System.ComponentModel.DataAnnotations;

namespace IntraNet.Models
{
    public class Processo
    {
        public int ProcessoId { get; set; }

        [Required]
        public string Titulo { get; set; } = "";

        [Required]
        public string Descricao { get; set; } = "";

        public string Setor { get; set; } = "";

        public DateTime DataCriacao { get; set; }
        public string ArquivoPath { get; set; } = "";

        public string AutorId { get; set; } = "";
    }
}
