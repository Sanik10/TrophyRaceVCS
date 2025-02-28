﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Enviro
{
    [CustomEditor(typeof(EnviroEffectsModule))]
    public class EnviroEffectsModuleEditor : EnviroModuleEditor
    {  
        private EnviroEffectsModule myTarget; 
      
        //On Enable
        public override void OnEnable()
        {
            if(!target)
                return; 

            myTarget = (EnviroEffectsModule)target;
            serializedObj = new SerializedObject(myTarget);
            preset = serializedObj.FindProperty("preset");
        
        }

        public override void OnInspectorGUI()
        {
            if(!target)
                return;

            base.OnInspectorGUI();

            GUI.backgroundColor = baseModuleColor;
            GUILayout.BeginVertical("",boxStyleModified);
            GUI.backgroundColor = Color.white;
            EditorGUILayout.BeginHorizontal();
            myTarget.showModuleInspector = GUILayout.Toggle(myTarget.showModuleInspector, "Effects", headerFoldout);
            
            GUILayout.FlexibleSpace();
            if(GUILayout.Button("x", EditorStyles.miniButtonRight,GUILayout.Width(18), GUILayout.Height(18)))
            {
                EnviroManager.instance.RemoveModule(EnviroManager.ModuleType.Effects);
                DestroyImmediate(this);
                return;
            } 
             
            EditorGUILayout.EndHorizontal();
            
            if(myTarget.showModuleInspector)
            {
                RenderDisableInputBox();
                serializedObj.UpdateIfRequiredOrScript ();
                EditorGUI.BeginChangeCheck();

                GUI.backgroundColor = categoryModuleColor;
                GUILayout.BeginVertical("",boxStyleModified);
                GUI.backgroundColor = Color.white;
                myTarget.showSetupControls = GUILayout.Toggle(myTarget.showSetupControls, "Setup", headerFoldout);              
                if(myTarget.showSetupControls)
                {
                    GUILayout.Space(10);
                    if (!Application.isPlaying) 
                    {
                        if (GUILayout.Button ("Add")) 
                        {
                            myTarget.Settings.effectTypes.Add (new EnviroEffectTypes ());
                        }
                    } 
                    else
                        EditorGUILayout.LabelField ("Can't add effects in runtime!");

                    if (GUILayout.Button ("Apply Changes")) 
                    {
                        myTarget.CreateEffects();
                    }

                    GUILayout.Space(10);
                    
                    for (int i = 0; i < myTarget.Settings.effectTypes.Count; i++) 
                    {      
                        GUILayout.BeginVertical (myTarget.Settings.effectTypes[i].name, boxStyleModified);
                        GUILayout.Space(15);
                        Undo.RecordObject(target, "Effect Setup Changed");
                        myTarget.Settings.effectTypes[i].name = EditorGUILayout.TextField ("Effect Name", myTarget.Settings.effectTypes[i].name);
                        myTarget.Settings.effectTypes[i].prefab = (GameObject)EditorGUILayout.ObjectField ("Effect Prefab", myTarget.Settings.effectTypes[i].prefab, typeof(GameObject), true);
                        myTarget.Settings.effectTypes [i].localPositionOffset = EditorGUILayout.Vector3Field ("Position Offset", myTarget.Settings.effectTypes [i].localPositionOffset);
                        myTarget.Settings.effectTypes [i].localRotationOffset = EditorGUILayout.Vector3Field ("Rotation Offset", myTarget.Settings.effectTypes [i].localRotationOffset);
                        GUILayout.Space(10);
                        myTarget.Settings.effectTypes [i].emissionRate = EditorGUILayout.Slider("Emission", myTarget.Settings.effectTypes [i].emissionRate,0f,1f);
                        myTarget.Settings.effectTypes [i].maxEmission = EditorGUILayout.FloatField ("Maximum Emission", myTarget.Settings.effectTypes [i].maxEmission);
 
                        if (GUILayout.Button ("Remove")) 
                        {
                            myTarget.Settings.effectTypes.Remove (myTarget.Settings.effectTypes[i]);
                        }
                        GUILayout.EndVertical ();
                    }

                }
                GUILayout.EndVertical();
               
                /// Save Load
                GUI.backgroundColor = categoryModuleColor;
                GUILayout.BeginVertical("",boxStyleModified);
                GUI.backgroundColor = Color.white;
                myTarget.showSaveLoad = GUILayout.Toggle(myTarget.showSaveLoad, "Save/Load", headerFoldout);
                
                if(myTarget.showSaveLoad)
                {
                    EditorGUILayout.PropertyField(preset);

                    GUILayout.BeginHorizontal("",wrapStyle);

                    if(myTarget.preset != null)
                    {
                        if(GUILayout.Button("Load"))
                        {
                            myTarget.LoadModuleValues();
                        }
                        if(GUILayout.Button("Save"))
                        {
                            myTarget.SaveModuleValues(myTarget.preset);
                        }
                    }
                    if(GUILayout.Button("Save As New"))
                    {
                        myTarget.SaveModuleValues();
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
                /// Save Load End

                ApplyChanges (); 
                EditorUtility.SetDirty (target);
            }
            GUILayout.EndVertical();

        if(myTarget.showModuleInspector)
            GUILayout.Space(20);
        }
    }
}
