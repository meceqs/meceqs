using System;
using System.Collections.Generic;
using Customers.Contracts.Commands;

namespace TrafficGenerator
{
    public static class CustomerFactory
    {
        private static Random random = new Random();

        public static CreateCustomerCommand GetRandomCreateCustomerCommand()
        {
            int randomIndex = random.Next(_names.Count);
            string[] randomName = _names[randomIndex].Split(' ');

            return new CreateCustomerCommand
            {
                FirstName = randomName[0],
                LastName = randomName[1]
            };
        }

        private static readonly List<string> _names = new List<string>
        {
            // generated with http://listofrandomnames.com/
            "Tania Threadgill",
            "Cedrick Seago",
            "Jennie Oliveras",
            "Talisha Tieman",
            "Diane Flick",
            "Eunice Liller",
            "Andera Bruner",
            "Karry Goranson",
            "Jamel Salone",
            "Sherrell Street",
            "Sharda Hern",
            "Berna Cowles",
            "Hyman Chrzanowski",
            "Chanel Marguez",
            "Tangela Bolenbaugh",
            "Lan Catto",
            "Francis Carolina",
            "Eugene Iwamoto",
            "Marietta Cordes",
            "Mckinley Zeller",
            "Rona Pouliot",
            "Marceline Ferrero",
            "Angelica Almonte",
            "Jin Covell",
            "Tequila Ewart",
            "Thora Corp",
            "Lynna Acres",
            "Cathi Bertolini",
            "Fernande Lothrop",
            "Vannesa Glisson",
            "Assunta Melin",
            "Elena Timpson",
            "Ilana Farias",
            "Deetta Oxner",
            "Stephany Marnell",
            "Lavern Drumm",
            "Herlinda Biermann",
            "Glennis Lampley",
            "Matilda Peed",
            "Nannette Boylan",
            "Willy Bourland",
            "Despina Milliken",
            "Lisha Mounsey",
            "Marquis Demming",
            "Naomi Mejorado",
            "Cliff Joerling",
            "Marry Reels",
            "Barb Haldeman",
            "Marissa Shurtliff",
            "Elsie Jinkins",
            "Letty Severance",
            "Brent Kaneshiro",
            "Holly Gott",
            "Scott Mckay",
            "Marjorie Friberg",
            "Callie Knopf",
            "Katlyn Quebedeaux",
            "Carman Monroy",
            "Seth Arbeiter",
            "Denese Ho",
            "Randell Passarelli",
            "Alethia Householder",
            "Mara Rudolph",
            "Elenora Roussell",
            "Virgina Galentine",
            "Mel Fackler",
            "Leif Borkholder",
            "Roxy Heisey",
            "Brook Millener",
            "Harriett Holz",
            "Floyd Hildreth",
            "Zaida Shaffer",
            "Doretha Grizzell",
            "Gregoria Stach",
            "Vernetta Swiger",
            "Russel Beres",
            "Myra Derossett",
            "Vincent Ledwell",
            "Millie Laprade",
            "Johnette Galindez",
            "Rosana Mattern",
            "Hannelore Vangilder",
            "Patience Heatherington",
            "Orpha Armour",
            "Angella Hudkins",
            "Esteban Ocheltree",
            "Ava Kroger",
            "Rick Jonson",
            "Cassey Sanroman",
            "Chrissy Ku",
            "Selina Leach",
            "Quentin Mceuen",
            "Alfreda Jones",
            "Sena Bentz",
            "Naoma Ontiveros",
            "Jeanne Lennon",
            "Carmela Law",
            "Anthony Walrath",
            "Jennifer Wurst",
            "Nicolle Mateer"
        };
    }
}