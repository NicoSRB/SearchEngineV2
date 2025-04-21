using System;
namespace Shared
{
    public class Paths
    {
        public static string FOLDER = @"/Users\brolo\Desktop\it-arkitektur\Semester-6\Arkitektur-principper\SearchEngineV2\Data\seData\medium";

        public static string DATABASE = @"/Users\brolo\Desktop\it-arkitektur\Semester-6\Arkitektur-principper\SearchEngineV2\Data\db.db";
        

        public static string FOLDERDB1 = @"/Users\brolo\Desktop\it-arkitektur\Semester-6\Arkitektur-principper\SearchEngineV2\Data\seData\large\allen-p";

        public static string DATABASEDB1 = @"/Users\brolo\Desktop\it-arkitektur\Semester-6\Arkitektur-principper\SearchEngineV2\Data\Allen.db";

        public static string FOLDERDB2 = @"/Users\brolo\Desktop\it-arkitektur\Semester-6\Arkitektur-principper\SearchEngineV2\Data\seData\large\baughman-d";

        public static string DATABASEDB2 = @"/Users\brolo\Desktop\it-arkitektur\Semester-6\Arkitektur-principper\SearchEngineV2\Data\Baughman.db";
    
        public static string  DatabasePathForDocker = "/Data/db.db";

        public static string FolderForDockerDB1 = "/Data/seData/medium";

        public static string GetDatabase(int id)
        {
            return id == 1 ? DATABASEDB1 : DATABASEDB2;
        }
    }
}
