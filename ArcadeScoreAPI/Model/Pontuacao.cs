using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations;

namespace ArcadeScoreAPI.Model
{
    [FirestoreData]
    public class Pontuacao
    {
        public string? Id { get; set; }  

        [FirestoreProperty]
        [Required]
        public string dt_Partida { get; set; }
        [FirestoreProperty]
        [Required]
        public int nr_pontuacao { get; set; }
        [FirestoreProperty]
        [Required]
        public string st_nome { get; set; }
    }
}
