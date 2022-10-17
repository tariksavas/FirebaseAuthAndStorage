namespace Incru.DataModels
{
    using Firebase.Firestore;

    [FirestoreData]
    public class UserInformation
    {
        [FirestoreProperty] public string userName { get; set; }
        [FirestoreProperty] public int classType { get; set; }
    }

    [FirestoreData]
    public class CharacterInformation
    {
        [FirestoreProperty] public string id { get; set; }
        [FirestoreProperty] public int type { get; set; }
        [FirestoreProperty] public int value { get; set; }
    }
}