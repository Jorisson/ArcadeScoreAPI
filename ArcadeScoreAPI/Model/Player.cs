﻿namespace ArcadeScoreAPI.Model
{
    public class Player
    {
        public string st_nome { get; set; }
        public int nr_jogadas { get; set; }
        public int nr_maior_pont { get; set; }
        public int nr_menor_pont { get; set; }
        public int nr_qtd_recorde { get; set; }
        public DateTime dt_primeiro_jogo { get; set; }
    }
}
