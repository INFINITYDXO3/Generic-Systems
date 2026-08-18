using UnityEditor;
using System;

[CustomEditor(typeof(WeaponData))]
public class WeaponDataCustomEditor : Editor
{
    private SerializedProperty weaponType;
    private SerializedProperty damage;
    private SerializedProperty recoilData;
    private SerializedProperty knockback;
    private SerializedProperty spreadAngle;
    private SerializedProperty range;
    private SerializedProperty fireRate;
    private SerializedProperty reloadTime;
    private SerializedProperty bulletType;
    private SerializedProperty magSize;

    private int weaponTypeIndex;

    private void OnEnable()
    {
        // Cache all properties once
        weaponType = serializedObject.FindProperty("WeaponType");
        damage = serializedObject.FindProperty("Damage");
        recoilData = serializedObject.FindProperty("RecoilData");       
        knockback = serializedObject.FindProperty("Knockback");       
        spreadAngle = serializedObject.FindProperty("SpreadAngle");
        range = serializedObject.FindProperty("Range");
        fireRate = serializedObject.FindProperty("FireRate");
        reloadTime = serializedObject.FindProperty("ReloadTime");
        bulletType = serializedObject.FindProperty("BulletType");
        magSize = serializedObject.FindProperty("MagSize");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        CustomFields();
    }

    private void CustomFields()
    {
        serializedObject.Update();

        weaponTypeIndex = weaponType.enumValueIndex;
        
        //Weapon Type
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Weapon Type: ");
        weaponTypeIndex = EditorGUILayout.Popup(weaponTypeIndex, Enum.GetNames(typeof(WeaponsTypes)));
        weaponType.enumValueIndex = weaponTypeIndex;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        //General Fields
        CustomEditorUtilities.NumberField(damage, "Damage");
        CustomEditorUtilities.NumberField(knockback, "Knockbak");

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("The Attack rate is the number of attacks per second");
        CustomEditorUtilities.NumberField(fireRate, "Attack Rate");

        EditorGUILayout.Space();

        //Gun Fields
        if((WeaponsTypes) weaponTypeIndex != WeaponsTypes.Melee)
        {
            // Extract nested fields from the struct using FindPropertyRelative
            SerializedProperty recoilX = recoilData.FindPropertyRelative("RecoilX");
            SerializedProperty recoilY = recoilData.FindPropertyRelative("RecoilY");
            SerializedProperty recoilZ = recoilData.FindPropertyRelative("RecoilZ");
            SerializedProperty kickbackZ = recoilData.FindPropertyRelative("KickbackZ");

            // Draw them
            CustomEditorUtilities.NumberField(recoilX, "Recoil X");
            CustomEditorUtilities.NumberField(recoilY, "Recoil Y");
            CustomEditorUtilities.NumberField(recoilZ, "Recoil Z");
            CustomEditorUtilities.NumberField(kickbackZ, "Kickback Z");
            
            CustomEditorUtilities.NumberField(spreadAngle, "Spread Angle");
            CustomEditorUtilities.NumberField(range, "Range");
            CustomEditorUtilities.NumberField(reloadTime, "Reload Time");
            CustomEditorUtilities.NumberField(magSize, "Mag Size");
        }
        
        serializedObject.ApplyModifiedProperties();
    }
}