using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Engine
{
    public delegate bool CheckExistance<T>(out T obj);
    public abstract class Math
    {
        /// <summary>
        /// Zwraca prawde lub falsz, jeśli nastąpiło zdarzenie, argument float od 0 do 1
        /// MAX 1!!!
        /// </summary>
        public static bool Probability(float probability)
        {
            return UnityEngine.Random.Range(0f, 1f) < probability ? true : false;
        }

        public static Vector3 DirectionVector(Vector3 startPoint, Vector3 destinationPoint)
        {
            var heading = destinationPoint - startPoint;
            var distance = heading.magnitude;
            return heading / distance;
        }

        public static Vector3 Direction(Vector3 startPoint, Vector3 destinationPoint)
        {
            return (destinationPoint - startPoint).normalized;
        }

        public static float SignAngle(Transform transform, Vector3 to)
        {
            float angle = Vector3.Angle(transform.forward, to);
            float rightAngle = Vector3.Angle(transform.right, to);
            float leftAngle = Vector3.Angle(-transform.right, to);
            if (leftAngle > rightAngle)
            {
                return angle;
            }
            else
            {
                return -angle;
            }
        }

        public static Vector3 QuaternionToVector(Quaternion quaternion)
        {
            return quaternion * Vector3.forward;
        }

        public static float SignAngle(ETransform transform, Vector3 to)
        {
            float angle = Vector3.Angle(transform.Forward, to);
            float rightAngle = Vector3.Angle(transform.Right, to);
            float leftAngle = Vector3.Angle(-transform.Right, to);
            if (leftAngle > rightAngle)
            {
                return angle;
            }
            else
            {
                return -angle;
            }
        }

        public static void MoveTowardsPointRotation(UnityEngine.Transform transform, Vector3 currentPosition, Vector3 targetPosition, float speed, float turnSpeed)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetPosition - currentPosition), turnSpeed);
            transform.position = transform.position + transform.forward * speed;
        }

        public static void RotateTowards(UnityEngine.Transform transform, Vector3 currentPosition, Vector3 targetPosition, float turnSpeed)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetPosition - currentPosition), Time.deltaTime * turnSpeed);
        }

        public static Quaternion RotateTowards(Vector3 currentPosition, Vector3 targetPosition)
        {
            return Quaternion.LookRotation(targetPosition - currentPosition);
        }

        public static Quaternion RotateTowards(UnityEngine.Transform transform, Vector3 targetPosition, float turnSpeed)
        {
            return Quaternion.LookRotation(targetPosition - transform.position);
        }

        public static Quaternion ReverseDirection(UnityEngine.Transform transform, Vector3 targetPosition)
        {
            return Quaternion.LookRotation(transform.position - targetPosition);
        }

        public static Quaternion RotateTowardsTopDown(UnityEngine.Transform transform, Vector3 target, float turnSpeed)
        {
            return Quaternion.Slerp(Quaternion.Euler(0, transform.eulerAngles.y, 0), Quaternion.LookRotation(target - transform.position), turnSpeed);
        }
        public static Quaternion RotateTowardsTopDown(UnityEngine.Transform transform, Vector3 target)
        {
            return Quaternion.Euler(0, Quaternion.LookRotation(target - transform.position).y,0);
        }


    }
    public abstract class Game : MonoBehaviour
    {
        public int ID
        {
            get
            {
                return GetHashCode();
            }
        }
        public sealed class GUI
        {
            /// <summary>
            /// Sprawdza, czy kliknięto na element w GUI, jeśli tak to zwraca true, jeśli nie to zwraca false (Dla pivot Center)
            /// </summary>
            public static bool OnRightClickGUI(RectTransform recTransform)
            {
                var width = recTransform.rect.width;
                var height = recTransform.rect.height;
                var x = recTransform.position.x;
                var y = recTransform.position.y;
                var mouseX = Input.mousePosition.x;
                var mouseY = Input.mousePosition.y;

                if ((mouseX > x - width / 2 && mouseX < x + width / 2) && (mouseY > y - height / 2 && mouseY < y + height / 2))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static IEnumerator WaitForFrames(int frames)
        {
            while (frames > 0)
            {
                frames--;
                yield return new WaitForEndOfFrame();
            }
        }

        /// <summary>
        /// Metoda zatrzymuje gre i zwraca tablice bool będącą indeksem dla skryptow w momencie pauzy
        /// tzn. czy byly aktywne czy nie, tablice tą należy przekazać do funkcji PauseLeave, aby wyjsc z pauzy
        /// </summary>
        /// <param name="root"> cały root w którym znajduje się ten objekt pozostanie aktywny, czyli jest to objekt typu Menu </param>
        /// <param name="array"> tablica powinna byc wczesniej zdefiniowana, poniewaz musi zostac pozniej uzyta</param>
        public static bool[] PauseEnter(UnityEngine.Transform root)
        {
            Time.timeScale = 0;
            MonoBehaviour[] scripts = FindObjectsOfType<MonoBehaviour>();
            bool[] array = new bool[scripts.Length];
            int i = 0;
            foreach (MonoBehaviour script in scripts)
            {
                array[i] = script.enabled;
                i++;
                if (script.transform.root != root)
                {
                    script.enabled = false;
                }
            }
            return array;

        }
        /// <summary>
        /// Metoda Wznawia gre
        /// 
        /// </summary>
        /// <param name="array"> tablica zapisana z metody PauseEnter</param>
        public static void PauseLeave(bool[] array)
        {
            Time.timeScale = 1;
            MonoBehaviour[] scripts = FindObjectsOfType<MonoBehaviour>();
            int i = 0;
            foreach (MonoBehaviour script in scripts)
            {
                script.enabled = array[i];
                i++;
            }
        }






        /// <summary>
        /// Sprawdza czy podana animacja aktualnie jest odtwarzana
        /// 
        /// </summary>
        /// <param name="anim">Animator na ktorym chcemy sprawdzic animacje</param>
        /// <param name="animationName">Animator.StringToHash("name")</param>
        /// /// <param name="layer">domyślnie (Base Layer) -> 0</param>
        public static bool AnimationIsPlaying(Animator anim, int animationName, int layer)
        {
            return anim.GetCurrentAnimatorStateInfo(layer).shortNameHash == animationName;
            //0 w GetCurrentAnimatorStateInfo to podstawowa warstwa czyli Base Layer
        }
    }

    public abstract class D2D : Game
    {

        public Quaternion RotateTowards(Quaternion rotation, Vector3 currentPosition, Vector3 targetPosition, float turnSpeed)
        {
            rotation = Quaternion.Slerp(rotation, Quaternion.LookRotation(targetPosition - currentPosition), Time.deltaTime * turnSpeed);
            return Quaternion.Euler(0, 0, rotation.eulerAngles.z);
        }




        public Quaternion Rotation2D(UnityEngine.Transform transform, Vector3 targetPosition)
        {
            Vector3 moveDirection = targetPosition - transform.position;
            if (moveDirection != Vector3.zero)
            {
                float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
                return Quaternion.AngleAxis(angle, Vector3.forward);
            }
            else
            {
                return Quaternion.identity;
            }
        }

        public enum LookRotation2D { Left = -1, Right = 1 }

        public LookRotation2D LookRotation
        {
            get
            {
                return transform.localScale.x > 0 ? LookRotation2D.Right : LookRotation2D.Left;
            }
            set
            {
                bool positive = value == LookRotation2D.Right ? true : false;
                Vector3 scale = transform.localScale;
                if (positive)
                {
                    scale = new Vector3(Mathf.Abs(scale.x), scale.y, scale.z);
                }
                else
                {
                    scale = new Vector3(Mathf.Abs(scale.x) * -1, scale.y, scale.z);
                }

                transform.localScale = scale;
            }
        }
    }
    public sealed class Mouse
    {
        /// <summary>
        /// Funkcja odnajduje pozycje myszki w swiecie 3D i zwraca vektor
        /// </summary>
        public static Vector3 GetMouse()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit groundHit;

            if (Physics.Raycast(ray, out groundHit, Mathf.Infinity, 1, QueryTriggerInteraction.Ignore))
            {
                return groundHit.point;
            }
            else
            {
                return new Vector3(0, 0, 0);
            }
        }


        /// <summary>
        /// Funkcja odnajduje pozycje myszki w swiecie 3D i zwraca vektor, Można manipulować pozycją myszki
        /// </summary>
        public static Vector3 GetMouseChanged(Vector2 mousePositionOnScreen)
        {
            Ray ray = Camera.main.ScreenPointToRay(mousePositionOnScreen);
            RaycastHit groundHit;

            if (Physics.Raycast(ray, out groundHit, Mathf.Infinity, 1, QueryTriggerInteraction.Ignore))
            {
                return groundHit.point;
            }
            else
            {
                return new Vector3(0, 0, 0);
            }
        }

        public static Vector3 GetMouse(Camera cam, int layer, QueryTriggerInteraction trigerInteraction)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit groundHit;

            if (Physics.Raycast(ray, out groundHit, Mathf.Infinity, layer, trigerInteraction))
            {
                return groundHit.point;
            }
            else
            {
                return new Vector3(0, 0, 0);
            }
        }

        /// <summary>
        /// Funkcja odnajduje pozycje myszki w swiecie 3D i zwraca vektor
        /// </summary>
        public static Vector3 GetMouseTag(string tag)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit[] rayHit = Physics.RaycastAll(ray);

            foreach (RaycastHit hit in rayHit)
            {
                if (hit.transform.tag == tag)
                {
                    return hit.point;
                }
            }
            return new Vector3(0, 0, 0);
        }
    }
    public sealed class Navigation
    {

        public static bool RandomNavPosition(Vector3 center, float range, out Vector3 result)
        {
            for (int i = 0; i < 30; i++)
            {
                Vector3 randomPoint = center + UnityEngine.Random.insideUnitSphere * range;
                UnityEngine.AI.NavMeshHit hit;
                if (UnityEngine.AI.NavMesh.SamplePosition(randomPoint, out hit, 1.0f, UnityEngine.AI.NavMesh.AllAreas))
                {
                    result = hit.position;
                    return true;
                }
            }
            result = center;
            return false;
        }
    }

    public sealed class D3D
    {
        public static Vector3 DirectionVector(Vector3 startPoint, Vector3 destinationPoint)
        {
            var heading = destinationPoint - startPoint;
            var distance = heading.magnitude;
            return heading / distance;
        }
    }

    public abstract class Instance : D2D
    {

        private static Dictionary<int, InstanceContainer> instancesAll = new Dictionary<int, InstanceContainer>();
        private static Dictionary<int, InstanceContainer> instancesActive = new Dictionary<int, InstanceContainer>();
        private static Dictionary<int, InstanceContainer> instancesDeactivated = new Dictionary<int, InstanceContainer>();



        private Rigidbody rb = null;
        public Rigidbody Rigidbody
        {
            get
            {
                if (rb != null)
                {
                    return rb;
                }
                else
                {
                    rb = GetComponent<Rigidbody>();
                    if (rb == null)
                    {
                        Debug.Log("This object doesn't contain Rigidbody component");
                    }
                    return rb;
                }
            }

            set
            {
                rb = value;
            }
        }

        public class InstanceContainer : Instance
        {
            public float deactivateDistance;
            public int team;

            public virtual void Awake()
            {
                instancesAll.Add(ID, this);
                if (gameObject.activeInHierarchy)
                {
                    instancesActive.Add(ID, this);
                }
                else
                {
                    instancesDeactivated.Add(ID, this);
                }
            }

            public virtual void OnEnable()
            {

                try
                {
                    instancesActive.Add(ID, this);
                    instancesDeactivated.Remove(ID);
                }
                catch { }
            }
            public virtual void OnDisable()
            {
                try
                {
                    instancesActive.Remove(ID);
                    InstancesDeactivated.Add(ID, this);
                }
                catch { }

            }

            public virtual void OnDestroy()
            {
                instancesAll.Remove(ID);
            }
        }

        //Position
        public float X
        {
            get
            {
                return transform.position.x;
            }
            set
            {
                transform.position += new Vector3(value, 0, 0);
            }
        }

        public float Y
        {
            get
            {
                return transform.position.y;
            }
            set
            {
                transform.position += new Vector3(0, value, 0);
            }
        }

        public float Z
        {
            get
            {
                return transform.position.z;
            }
            set
            {
                transform.position += new Vector3(0, 0, value);
            }
        }


        /// <summary>
        /// Sprawdza czy jest kolizja na lini pomiedzy wektorami, jesli nie ma zwraca NULL
        /// </summary>
        public static UnityEngine.Transform GetObjectInCollisionLine(Vector3 startPosition, Vector3 direction, float distanceToCheck)
        {
            UnityEngine.Transform obj = null;
            RaycastHit hit;
            if (Physics.Raycast(startPosition, direction, out hit, distanceToCheck))
            {
                obj = hit.transform;
            }
            return obj;
        }



        /// <summary>
        /// Odnajduje najbliższą pustą pozycję od vektora startPos, przeskakuje o wartość cellSize i sprawdza czy trafiła ziemie a nie co innego, ziemią jest groundTag
        /// </summary>
        public static Vector3 FindEmpty(Vector3 startPos, int cellSize, string groundTag)
        {
            var pos = startPos;
            UnityEngine.Transform obj = Camera.main.transform; //Jakiś obiekt musi być przypisany, a kamera zawsze jest na scenie :P
            float x = -2;
            float z = 0;
            float y = pos.y + 20;
            x *= cellSize;

            //Position


            while (obj.tag != groundTag)
            {
                pos = new Vector3(startPos.x + x, y, startPos.z - z);
                obj = Instance.GetObjectInCollisionLine(pos, Vector3.down, 100);
                if (x < 3 * cellSize)
                {
                    x += cellSize;
                }
                else
                {
                    x = -2 * cellSize;
                    z -= cellSize;
                }
                pos.y -= 20;
            }
            return pos;
        }

        /// <summary>
        /// Zwraca liczbe Componentow <T> na scenie.
        /// </summary>
        ///   
        public static List<T> SceneComponents<T>()
        {
            var allObjects = UnityEngine.Object.FindObjectsOfType<UnityEngine.Object>();
            var list = new List<T>();
            foreach (UnityEngine.Object obj in allObjects)
            {
                if (obj.GetType() == typeof(T))
                {
                    list.Add((T)System.Convert.ChangeType(obj, typeof(T)));
                }
            }
            return list;
        }

        /// <summary>
        /// Zwraca liczbe Componentow <T> w game object.
        /// </summary>
        ///   
        public T[] InstanceGetComponents<T>()
        {
            return GetComponentsInChildren<T>();
        }

        /// <summary>
        /// Znajduje najblizszy objekt do wprowadzonej pozycji. Wolniejsze od ObjectNearestGiven
        /// </summary>
        /// <param name="position"> Pozycja od ktorej znajdujemy najblizszy objekt </param>
        /// <param name="tag"> "Tag" objektu z inpsektora </param>
        public UnityEngine.Transform ObjectNearestTag(Vector3 position, string tag)
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
            if (objects.Length > 0)
            {
                float distance = Mathf.Infinity;
                UnityEngine.Transform instance = instancesAll[0].transform;
                foreach (GameObject obj in objects)
                {
                    float dis = Vector3.Distance(position, obj.transform.position);
                    if (dis < distance)
                    {
                        distance = dis;
                        instance = obj.transform;
                    }
                }
                return instance;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Znajduje najblizszy objekt z tablicy podanych objektow do wprowadzonej pozycji. Wolniejsze od InstanceNearestGiven
        /// </summary>
        /// <param name="position"> Pozycja od ktorej znajdujemy najblizszy objekt </param>
        /// <param name="tag"> "Tag" objektu z inpsektora </param>
        public UnityEngine.Transform ObjectNearestGiven(Vector3 position, UnityEngine.Transform[] objects)
        {
            if (objects.Length > 0)
            {
                float distance = Mathf.Infinity;
                UnityEngine.Transform instance = objects[0];
                foreach (UnityEngine.Transform trans in objects)
                {
                    float dis = Vector3.Distance(position, trans.position);
                    if (dis < distance)
                    {
                        distance = dis;
                        instance = trans;
                    }
                }
                return instance;
            }
            else
            {
                Debug.Log("Tablica objektow jest pusta.");
                return null;
            }
        }

        /// <summary>
        /// Znajduje najblizszy objekt z Listy podanych objektow do wprowadzonej pozycji. Wolniejsze od InstanceNearestGiven
        /// </summary>
        /// <param name="position"> Pozycja od ktorej znajdujemy najblizszy objekt </param>
        /// <param name="tag"> "Tag" objektu z inpsektora </param>
        public UnityEngine.Transform InstanceNearestGivenList(Vector3 position, List<UnityEngine.Transform> instances)
        {
            if (instances.Count > 0)
            {
                float distance = Mathf.Infinity;
                UnityEngine.Transform instance = instances[0];
                foreach (UnityEngine.Transform trans in instances)
                {
                    float dis = Vector3.Distance(position, trans.position);
                    if (dis < distance)
                    {
                        distance = dis;
                        instance = trans;
                    }
                }
                return instance;
            }
            else
            {
                return null;
            }
        }

        private static int curIndex = 0;

        //W trakcie każdego Update sprawdza max 5 obiektow
        public static void Deactivate(UnityEngine.Transform instance)
        {
            int loop = 0;
            int index = curIndex;
            foreach (InstanceContainer ii in instancesAll.Values)
            {
                if (index > 0)
                {
                    index--;
                }
                else
                {
                    if (ii.gameObject.activeInHierarchy && Vector3.Distance(instance.position, ii.transform.position) > ii.deactivateDistance)
                    {
                        ii.gameObject.SetActive(false);
                        instancesActive.Remove(ii.ID);
                        instancesDeactivated.Add(ii.ID, ii);
                    }

                    if (!ii.gameObject.activeInHierarchy && Vector3.Distance(instance.position, ii.transform.position) < ii.deactivateDistance)
                    {
                        ii.gameObject.SetActive(true);
                        instancesDeactivated.Remove(ii.ID);
                        instancesActive.Add(ii.ID, ii);
                    }
                    loop++;
                    if (loop == 5)
                    {
                        break;
                    }
                }
            }
            curIndex = curIndex < instancesAll.Count - 5 ? curIndex + 5 : 0;

        }

        /// <summary>
        /// Znajduje najblizszy objekt do wprowadzonej pozycji. Wolniejsze od InstanceNearestTag
        /// </summary>
        /// <param name="position"> Pozycja od ktorej znajdujemy najblizszy objekt </param>
        /// <param name="tag"> "Tag" objektu z inpsektora </param>
        public static UnityEngine.Transform ObjectsNearestAll(Vector3 position)
        {
            GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
            if (allObjects.Length > 0)
            {
                float distance = Mathf.Infinity;
                UnityEngine.Transform instance = allObjects[0].transform;
                foreach (GameObject obj in allObjects)
                {
                    float dis = Vector3.Distance(position, obj.transform.position);
                    if (dis < distance)
                    {
                        distance = dis;
                        instance = obj.transform;
                    }
                }
                return instance;
            }
            else
            {
                Debug.Log("Tablica objektow jest pusta.");
                return null;
            }
        }

        /// <summary>
        /// Znajduje najdalsza pozycje z tablicy Vectorow do podanej pozycji startowej
        /// </summary>
        /// <param name="startPos"> Pozycja od ktorej znajdujemy najdalszy Vector3 </param>
        /// <param name="positions"> Tablica Vector3 z ktorej znajdziemy najdalszy Vector3 do startPos </param>
        public static Vector3 ObjectFurthest(Vector3 startPos, Vector3[] positions)
        {
            Vector3 position = startPos;
            float distance = 0;
            foreach (Vector3 pos in positions)
            {
                float dis = Vector3.Distance(startPos, pos);
                if (dis > distance)
                {
                    distance = dis;
                    position = pos;
                }
            }
            return position;
        }

        /// <summary>
        /// Zwraca liczbe objektow na scenie.
        /// </summary>
        /// 

        public Dictionary<int, InstanceContainer> Instances
        {
            get
            {
                return instancesAll;
            }
        }
        public Dictionary<int, InstanceContainer> InstancesDeactivated
        {
            get
            {
                return instancesDeactivated;
            }
        }
        public Dictionary<int, InstanceContainer> InstancesActive
        {
            get
            {
                return instancesActive;
            }
        }
        public int InstancesCount
        {
            get
            {
                return instancesAll.Count;
            }
        }

        public int AllObjectsCount
        {
            get
            {
                return FindObjectsOfType<GameObject>().Length;
            }
        }

        public Component[] InstanceComponents<T>()
        {
            return GetComponents(typeof(T));
        }
    }

    public sealed class Draw
    {
        /// <summary>
        /// Rysuje teksture na GUI przycieta w poziomi i pionie, uzywac w void OnGUI
        /// 
        /// </summary>
        /// <param name="x">Pozycja x na ekranie</param>
        /// <param name="y">Pozycja y na ekranie</param>
        /// <param name="w">szerokość obrazu</param>
        /// <param name="h">wysokoścć obrazu</param>
        /// <param name="hCrop">przycięcie w poziomie minusowe i dodatnie</param>
        /// <param name="vCrop">przycięcie w pionie minusowe i dodatnie</param>
        /// <param name="tex">tekstura która ma być narysiwana</param>
        public static void DrawTexturePart(int x, int y, int w, int h, int hCrop, int vCrop, Texture tex)
        {
            Rect rect = new Rect(new Vector2(hCrop, vCrop), new Vector2(w, h));
            Rect outterRect = new Rect(new Vector2(x - hCrop, y - vCrop), new Vector2(w, h));
            UnityEngine.GUI.BeginGroup(outterRect);
            UnityEngine.GUI.DrawTexture(rect, tex, ScaleMode.ScaleAndCrop);
            UnityEngine.GUI.EndGroup();
        }
        public static float deltaTime = 0.0F;
        public static void DisplayFps(float x, float y, byte r, byte g, byte b, float a)
        {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            int w = Screen.width, h = Screen.height;

            GUIStyle style = new GUIStyle();

            Rect rect = new Rect(x, y, w, h * 2 / 100);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = h * 2 / 100;
            style.normal.textColor = new Colour(r, g, b, a);
            float msec = deltaTime * 1000.0f;
            fps = 1.0f / deltaTime;
            string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
            UnityEngine.GUI.Label(rect, text, style);
        }
        public static float fps;
        public static void DisplayFps(int x, int y, Colour color, int fontSize)
        {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            int w = Screen.width;

            GUIStyle style = new GUIStyle
            {
                fontSize = fontSize,
                alignment = TextAnchor.UpperLeft
            };
            style.normal.textColor = color;
            float msec = deltaTime * 1000.0f;
            fps = 1.0f / deltaTime;
            string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
            Rect rect = new Rect(x - (fontSize * text.Length/4), y, w, fontSize);
            UnityEngine.GUI.Label(rect, text, style);
        }
        static float medianFps;
        static int frame;
        public static void DisplayFpsMedian(int x, int y, Colour color, int fontSize)
        {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            int w = Screen.width;

            GUIStyle style = new GUIStyle
            {
                fontSize = fontSize,
                alignment = TextAnchor.UpperLeft
            };
            style.normal.textColor = color;
            float msec = deltaTime * 1000.0f;
            fps = 1.0f / deltaTime;
            frame++;
            if(frame %60==0)
            {
                frame = 0;
                medianFps = fps;
            }
            string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, medianFps);
            Rect rect = new Rect(x - (fontSize * text.Length / 4), y, w, fontSize);
            UnityEngine.GUI.Label(rect, text, style);
        }
        public static void DisplayFpsDebug(float x, float y, byte r, byte g, byte b, float a, bool logDropDowns)
        {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            int w = Screen.width, h = Screen.height;

            GUIStyle style = new GUIStyle();

            Rect rect = new Rect(x, y, w, h * 2 / 100);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = h * 2 / 100;
            style.normal.textColor = new Colour(r, g, b, a);
            float msec = deltaTime * 1000.0f;
            float fps = 1.0f / deltaTime;
            if (fps <= 59)
            {
                style.normal.textColor = new Colour(255, 0, 0, a);
                if (logDropDowns)
                {
                    Debug.Log("FPS drop to: " + fps);
                }

            }
            string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
            UnityEngine.GUI.Label(rect, text, style);
        }

        public static void ResetMedianFps()
        {
            fpsMedian = 0;
        }
        static double fpsMedian = 0;
        static long frames = 0;
        public static void DisplayMedianFps(float x, float y)
        {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            int w = Screen.width, h = Screen.height;

            GUIStyle style = new GUIStyle();

            Rect rect = new Rect(x, y, w, h * 2 / 100);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = h * 2 / 50;
            style.normal.textColor = Color.red;
            float msec = deltaTime * 1000.0f;
            fps = 1.0f / deltaTime;
            fpsMedian += fps;
            frames++;
            string text = string.Format("Median FPS: ({0:0.})", fpsMedian / frames);
            UnityEngine.GUI.Label(rect, text, style);
        }




        //DRAW TEXT
        /// <summary>
        /// Wyświetla tekst na ekranie, czarny ( uzywamy w OnGUI )
        /// </summary>
        /// <param name="x"> Pozycja x na ekranie </param>
        /// <param name="y"> pozycja y na ekranie </param>
        /// <param name="text"> tekst </param>
        public static void Text(int x, int y, object text)
        {
            var w = Screen.width;
            var h = Screen.height;

            GUIStyle style = new GUIStyle();

            Rect rect = new Rect(x, y, w, h * 2 / 100);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = h * 2 / 100;
            style.normal.textColor = new Colour(0, 0, 0, 1);
            UnityEngine.GUI.Label(rect, string.Format("{0}", text), style);
        }

        /// <summary>
        /// Wyświetla tekst na ekranie, czarny + rozmiar czcionki  ( uzywamy w OnGUI )
        /// </summary>
        /// <param name="x"> Pozycja x na ekranie </param>
        /// <param name="y"> pozycja y na ekranie </param>
        /// <param name="fontSize"> rozmiar czcionki </param>
        /// <param name="text"> tekst </param>
        public static void TextSize(int x, int y, int fontSize, string text)
        {
            var w = Screen.width;

            GUIStyle style = new GUIStyle();

            Rect rect = new Rect(x, y, w, fontSize);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = fontSize;
            style.normal.textColor = new Colour(0, 0, 0, 1);
            UnityEngine.GUI.Label(rect, string.Format("{0}", text), style);
        }

        /// <summary>
        /// Wyświetla tekst na ekranie, kolorowy + alpha  ( uzywamy w OnGUI )
        /// </summary>
        /// <param name="x"> Pozycja x na ekranie </param>
        /// <param name="y"> pozycja y na ekranie </param>
        /// <param name="red"> zawartosc kolora czerwonego 0-255</param>
        /// <param name="green"> zawartosc kolora zielonego 0-255</param>
        /// <param name="blue"> zawartosc kolora niebieskiego 0-255</param>
        /// <param name="alpha"> przezroczystosc 0-1F</param>
        /// <param name="text"> tekst </param>
        public static void TextColor(int x, int y, byte red, byte green, byte blue, float alpha, object text)
        {
            var w = Screen.width;
            var h = Screen.height;

            GUIStyle style = new GUIStyle();

            Rect rect = new Rect(x, y, w, h * 2 / 100);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = h * 2 / 100;
            style.normal.textColor = new Colour(red / 255f, green / 255f, blue / 255f, alpha);
            UnityEngine.GUI.Label(rect, string.Format("{0}", text), style);
        }

        public static void TextColorUnity(int x, int y, Colour color, object text)
        {
            var w = Screen.width;
            var h = Screen.height;

            GUIStyle style = new GUIStyle();

            Rect rect = new Rect(x, y, w, h * 2 / 100);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = h * 2 / 100;
            style.normal.textColor = color;
            UnityEngine.GUI.Label(rect, string.Format("{0}", text), style);
        }
        /// <summary> 
        /// Wyświetla tekst na ekranie, kolorowy + alpha + rozmiar  ( uzywamy w OnGUI )
        /// </summary>
        /// <param name="x"> Pozycja x na ekranie </param>
        /// <param name="y"> pozycja y na ekranie </param>
        /// <param name="red"> zawartosc kolora czerwonego 0-255</param>
        /// <param name="green"> zawartosc kolora zielonego 0-255</param>
        /// <param name="blue"> zawartosc kolora niebieskiego 0-255</param>
        /// <param name="alpha"> przezroczystosc 0-1F</param>
        /// <param name="fontSize"> rozmiar czcionki </param>
        /// <param name="text"> tekst </param>
        public static void TextColorSize(int x, int y, byte red, byte green, byte blue, float alpha, int fontSize, object text)
        {
            var w = Screen.width;

            GUIStyle style = new GUIStyle();

            Rect rect = new Rect(x, y, w, fontSize);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = fontSize;
            style.normal.textColor = new Colour(red / 255f, green / 255f, blue / 255f, alpha);
            UnityEngine.GUI.Label(rect, string.Format("{0}", text), style);
        }

        public static void TextColorSizeFont(int x, int y, byte red, byte green, byte blue, float alpha, int fontSize, object text, Font font)
        {
            var w = Screen.width;

            GUIStyle style = new GUIStyle();

            Rect rect = new Rect(x, y, w, fontSize);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = fontSize;
            style.normal.textColor = new Colour(red / 255f, green / 255f, blue / 255f, alpha);
            style.font = font;
            UnityEngine.GUI.Label(rect, string.Format("{0}", text), style);
        }
        /// <summary>
        /// Wyświetla tekst na ekranie, kolorowy + alpha + rozmiar + dopasowanie do wybranej strony  ( uzywamy w OnGUI )
        /// </summary>
        /// <param name="x"> X position on screen </param>
        /// <param name="y"> Y position on screen </param>
        /// <param name="red"> red value 0-255</param>
        /// <param name="green"> green value 0-255</param>
        /// <param name="blue"> blue value 0-255</param>
        /// <param name="alpha"> przezroczystosc 0-1F</param>
        /// <param name="fontSize"> text font size </param>
        /// <param name="align"> 8 possibilities: "UpperLeft" , "UpperCenter" , "UpperRight" , "MiddleLeft" , "MiddleCenter" , "MiddleRight" , "LowerLeft" , "LowerCenter" </param>
        /// <param name="text"> text </param>
        public static void TextColorSizeAlignFont(int x, int y, byte red, byte green, byte blue, float alpha, int fontSize, TextAnchor align, Font font, object text) // UpperLeft, UpperCenter, UpperRight,  
        {
            var w = Screen.width;

            GUIStyle style = new GUIStyle()
            {
                font = font,
                alignment = align
            };
            Rect rect = new Rect(x, y, w, fontSize);


            style.fontSize = fontSize;
            style.normal.textColor = new Colour(red / 255f, green / 255f, blue / 255f, alpha);
            UnityEngine.GUI.Label(rect, string.Format("{0}", text), style);
        }
    }



    public class ETransform
    {
        public Vector3 LocalPosition { get; set; }
        public Vector3 LocalScale { get; set; }
        public Quaternion LocalRotation { get; set; }
        public Vector3 LocalEulerAngles { get; private set; }
        public Vector3 Right { get; private set; }
        public Vector3 Forward { get; private set; }

        public static implicit operator ETransform(Transform trans)
        {
            return new ETransform()
            {
                LocalPosition = trans.localPosition,
                LocalScale = trans.localScale,
                LocalRotation = trans.localRotation,
                LocalEulerAngles = trans.localEulerAngles,
                Right = trans.right,
                Forward = trans.forward
            };
        }
    }

    [Serializable]
    public struct Vector
    {
        public const float kEpsilon = 1E-05F;
        public float x;
        public float y;
        public float z;
        public float w;

        public Vector(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            w = 0;
        }
        public Vector(float x, float y)
        {
            this.x = x;
            this.y = y;
            z = 0;
            w = 0;
        }
        public Vector(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public override string ToString()
        {
            return string.Format("({0},{1},{2})", x, y, z);
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }

        public Vector2 ToVector2()
        {
            return new Vector2(x, y);
        }

        public static implicit operator Vector(Vector3 a)
        {
            return new Vector(a.x, a.y, a.z);
        }

        public static Vector3 Direction(Vector3 start, Vector3 destination)
        {
            return (destination - start).normalized;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static Vector operator +(Vector a, Vector b)
        {
            return new Vector(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Vector operator -(Vector a, Vector b)
        {
            return new Vector(a.x - b.x, a.y - b.y, a.z - b.z);
        }
        public static Vector operator *(Vector a, float d)
        {
            return new Vector(a.x *d, a.y *d, a.z *d);
        }
        public static Vector operator *(float d, Vector a)
        {
            return new Vector(a.x * d, a.y * d, a.z * d);
        }
        public static Vector operator /(Vector a, float d)
        {
            return new Vector(a.x / d, a.y / d, a.z / d);
        }

        public static implicit operator Vector(Quaternion q)
        {
            return new Vector(q.x, q.y, q.z, q.w);
        }

        public static implicit operator Quaternion(Vector vec)
        {
            return new Quaternion(vec.x, vec.y, vec.z, vec.w);
        }
        public static implicit operator Vector3(Vector vec)
        {
            return new Vector3(vec.x, vec.y, vec.z);
        }
        public static implicit operator Vector2(Vector vec)
        {
            return new Vector2(vec.x, vec.y);
        }
    }

    [Serializable]
    public struct Float2
    {
        public float x;
        public float y;
        public Float2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        public static implicit operator Vector2(Float2 vec)
        {
            return new Vector2(vec.x, vec.y);
        }
        public static implicit operator Float2(Vector2 vec)
        {
            return new Float2(vec.x, vec.y);
        }
        public override string ToString()
        {
            return "("+x+","+y+")";
        }
    }

    [Serializable]
    public struct Float3
    {
        public float x;
        public float y;
        public float z;
        public Float3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public static implicit operator Vector3(Float3 vec)
        {
            return new Vector3(vec.x, vec.y, vec.z);
        }

        public static implicit operator Float3(Vector3 vec)
        {
            return new Float3(vec.x, vec.y, vec.z);
        }
    }

    [Serializable]
    public struct Float4
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public Float4(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }
        public static implicit operator Quaternion(Float4 vec)
        {
            return new Quaternion(vec.x, vec.y, vec.z, vec.w);
        }

        public static implicit operator Float4(Quaternion vec)
        {
            return new Float4(vec.x, vec.y, vec.z, vec.w);
        }
    }

    [Serializable]
    public struct Colour
    {
        public float r;
        public float g;
        public float b;
        public float a;

        public Colour(float r, float g, float b, float a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }
        public static implicit operator UnityEngine.Color(Engine.Colour col)
        {
            return new UnityEngine.Color(col.r,col.g,col.b,col.a);
        }

        public static implicit operator Engine.Colour(UnityEngine.Color col)
        {
            return new Engine.Colour(col.r, col.g, col.b, col.a);
        }
    }


    public static class ExtensionMethods
    {
        public static void ResetTransformation(this Transform trans)
        {
            trans.localPosition = Vector3.zero;
            trans.localRotation = Quaternion.identity;
            trans.localScale = new Vector3(1, 1, 1);
        }

        public static Vector3 Direction(Vector3 start, Vector3 destination)
        {
            return (destination - start).normalized;
        }

        private static System.Random rng = new System.Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static void Shuffle<T>(this T[] array)
        {
            int n = array.Length;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = array[k];
                array[k] = array[n];
                array[n] = value;
            }
        }

        public static Vector3 Vector(this Quaternion q)
        {
            return q * Vector3.forward;
        }

        //public static Vector ToVector(this Vector3 vector)
        //{
        //    return new Vector(vector.x, vector.y, vector.z);
        //}

        //public static Vector ToVector(this Vector2 vector)
        //{
        //    return new Vector(vector.x, vector.y, 0);
        //}

        //public static Vector ToVector(this Quaternion quartenion)
        //{
        //    return new Vector(quartenion.x, quartenion.y, quartenion.z, quartenion.w);
        //}

        public static void ToETransform(this Transform trans, ETransform eTrans)
        {
            trans.localPosition = eTrans.LocalPosition;
            trans.localRotation = eTrans.LocalRotation;
            trans.localScale = eTrans.LocalScale;
            trans.localEulerAngles = eTrans.LocalEulerAngles;
            trans.forward = eTrans.Forward;
            trans.right = eTrans.Right;
        }
    }



    public abstract class EngineThread
    {
        private bool m_IsDone = false;
        private object m_Handle = new object();
        protected System.Threading.Thread m_Thread = null;
        public delegate void Finish(object sender);
        public static event Finish OnFinished;
        public System.Threading.Thread Thread
        {
            get
            {
                return m_Thread;
            }
        }

        public bool IsDone
        {
            get
            {
                bool tmp;
                lock (m_Handle)
                {
                    tmp = m_IsDone;
                }
                return tmp;
            }
            set
            {
                lock (m_Handle)
                {
                    m_IsDone = value;
                }
            }
        }

        public EngineThread()
        {
            m_Thread = new System.Threading.Thread(Run);
            m_Thread.Start();
        }

        public virtual void Abort()
        {
            m_Thread.Abort();
        }

        protected abstract void ThreadFunction();

        protected abstract void Finished();

        public virtual bool Update()
        {
            if (IsDone)
            {
                Finished();
                return true;
            }
            return false;
        }

        private void Run()
        {
            ThreadFunction();
            OnFinished?.Invoke(this);
            IsDone = true;
        }
    }
    

}








