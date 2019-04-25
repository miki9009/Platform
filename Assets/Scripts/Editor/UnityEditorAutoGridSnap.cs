using UnityEngine;

using UnityEditor;



[InitializeOnLoad]
public class UnityEditorAutoGridSnap

{

    private static bool autoSnap = false;

    private static Vector3 prevTrPos = Vector3.zero;



    static UnityEditorAutoGridSnap()

    {

        SetSnapping(EditorPrefs.GetBool("grid.autosnap", autoSnap));

    }



    [MenuItem("Edit/Toggle Auto-Snap %_l")]

    private static void ToggleSnap()

    {

        SetSnapping(!autoSnap);

    }



    private static void SetSnapping(bool on)

    {

        autoSnap = on;

        EditorPrefs.SetBool("grid.autosnap", autoSnap);

        EditorApplication.update -= OnUpdate;

        if (on) EditorApplication.update += OnUpdate;

    }



    private static void OnUpdate()

    {

        if (!Application.isPlaying && Selection.transforms.Length > 0 && Selection.transforms[0].position != prevTrPos)

        {

            prevTrPos = Selection.transforms[0].position;

            Vector3 snap = SnapSettingsMove();

            foreach (Transform tr in Selection.transforms)

            {

                Vector3 pos = tr.position;

                if (snap.x > 0.0f) pos.x = Mathf.Round(pos.x / snap.x) * snap.x;

                if (snap.y > 0.0f) pos.y = Mathf.Round(pos.y / snap.y) * snap.y;

                if (snap.z > 0.0f) pos.z = Mathf.Round(pos.z / snap.z) * snap.z;

                tr.position = pos;

            }

        }

    }



    private static System.Func<Vector3> Invoke_SnapSettingsMove = null;

    private static Vector3 SnapSettingsMove()

    {

        if (Invoke_SnapSettingsMove == null)

        {

            System.Reflection.PropertyInfo p = typeof(EditorUtility).Assembly.GetType("UnityEditor.SnapSettings").GetProperty("move", (System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public));

            System.Reflection.MethodInfo m = p.GetGetMethod();

            Invoke_SnapSettingsMove = (System.Func<Vector3>)System.Delegate.CreateDelegate(typeof(System.Func<Vector3>), m);

        }

        return Invoke_SnapSettingsMove();

    }

}