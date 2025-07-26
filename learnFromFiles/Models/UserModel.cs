using System.ComponentModel.DataAnnotations;

namespace learnFromFiles.Models
{
    public class UserModel
    {
         [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; } = "";
        public string Greeting { get; set; } = "";
        public UserModel()
        {
            Name = "";
            Greeting="";
        }
        public UserModel(string name, string greeting)
        {

            Name = name;
            Greeting = greeting;
        }
    }

}
