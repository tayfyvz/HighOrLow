using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CardsWeightConfig))]
public class DeckWeightConfigEditor : Editor 
{
    SerializedProperty defaultCardWeight;
    SerializedProperty suitOverrides;
    SerializedProperty cardOverrides;

    private void OnEnable() {
        defaultCardWeight = serializedObject.FindProperty("DefaultWeight");
        suitOverrides = serializedObject.FindProperty("SuitOverrides");
        cardOverrides = serializedObject.FindProperty("CardOverrides");
    
        if (defaultCardWeight == null)
            Debug.LogError("DefaultWeight property not found!");
        if (suitOverrides == null)
            Debug.LogError("SuitOverrides property not found!");
        if (cardOverrides == null)
            Debug.LogError("CardOverrides property not found!");
    }


    public override void OnInspectorGUI() {
        serializedObject.Update();

        // Header for the configuration
        EditorGUILayout.LabelField("Deck Configuration", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(defaultCardWeight);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Suit Overrides", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(suitOverrides, true);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Card Overrides", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(cardOverrides, true);

        serializedObject.ApplyModifiedProperties();
    }
}