using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject chính dùng cho localization (đa ngôn ngữ)
/// </summary>
[CreateAssetMenu(fileName = "New string", menuName = "Loc. String")]
public class LocalizableString : ScriptableObject
{
    [Header("Translations")]
    public string english;
    public string vietnamese;

    /// <summary>
    /// Trả về chuỗi tương ứng với ngôn ngữ được chọn
    /// </summary>
    public string GetString(Language language)
    {
        switch (language)
        {
            case Language.English:
                return english;
            case Language.Vietnamese:
                return vietnamese;
        }

        return null;
    }

    [System.Serializable]
    public enum Language { English, Vietnamese };
}