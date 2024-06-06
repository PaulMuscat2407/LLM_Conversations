using UnityEngine;

public enum CharacterType
{
    Elijah_Thorne_Inventor,
    Inspector_Rupert_Blackwell,
    Lady_Constance_Fairchild_Spiritualist
}

public enum LocationType
{
    Victorian_London_Street_Market,
    Victorian_Steampunk_Workshop,
    Victorian_Psychic_Medium_Caravan
}

[CreateAssetMenu(fileName = "NewCharacterLocation", menuName = "CharacterLocation")]
public class CharacterLocation : ScriptableObject
{
    public CharacterType character;
    public LocationType location;
}
