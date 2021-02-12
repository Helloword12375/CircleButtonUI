using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CircleSelectorUI : MonoBehaviour
{
    public float index;
    public GameObject button;
    private Transform buttonTransform;
    private Transform smallImage;
    private GameObject showImage;
    private Image cursor;
    private float distance;

    public enum ControlType { mouseAndTouch, gamepad }
    public ControlType controlType;
    private string gamepadAxisX, gamepadAxisY;

    private List<GameObject> buttonList = new List<GameObject>();

    private void Start()
    {
        buttonTransform = transform.Find("Buttons");
        smallImage = transform.Find("SmallBG");
        showImage = transform.Find("ShowBG").gameObject;

        cursor = showImage.GetComponent<Image>();


        float Bigdistance = showImage.GetComponent<RectTransform>().rect.width * showImage.transform.localScale.x / 2;
        float Smalldistance = smallImage.GetComponent<RectTransform>().rect.width * smallImage.transform.localScale.x / 2;
        float coreDistance = Bigdistance - Smalldistance;
        distance = Bigdistance- coreDistance / 2;
    }
    public void RefreshUI() {
        buttonList.Clear();
        for (int j = 0; j < buttonTransform.childCount; j++) {
            Destroy(buttonTransform.GetChild(j).gameObject);
        }
        showImage.GetComponent<Image>().fillAmount = 1 / index;
        float fillRadius = (1 / index) * 360f;
        float previousRotation = 0;
        for (int i = 0; i < index; i++) {
            float bRot = previousRotation + fillRadius / 2;
            previousRotation = bRot + fillRadius / 2;
            GameObject go = Instantiate(button, buttonTransform);
            go.transform.localPosition = new Vector2(distance * Mathf.Cos((bRot - 90) * Mathf.Deg2Rad), -distance * Mathf.Sin((bRot - 90) * Mathf.Deg2Rad));
            if (bRot > 360)
                bRot -= 360;
            go.name = bRot.ToString();
            buttonList.Add(go);
        }
    }
    private void Update()
    {
        if (buttonList.Count == 0) return;
        Vector3 screenBounds = new Vector3((float)Screen.width / 2f, (float)Screen.height / 2f, 0f);
        Vector3 vector = Input.mousePosition - screenBounds;
      
        float mouseRotation = ((controlType == ControlType.mouseAndTouch) ? Mathf.Atan2(vector.x, vector.y) : Mathf.Atan2(Input.GetAxis(gamepadAxisX), Input.GetAxis(gamepadAxisY))) * 57.29578f;
        if (mouseRotation < 0f)
            mouseRotation += 360f;

        GameObject nearest = null;
        string SelectedSegment;
        float difference = 999;
        for (int i = 0; i < buttonList.Count; i++)
        {
            GameObject b;
            b = buttonList[i];
            float rotation = System.Convert.ToSingle(b.name);// - 360 / buttonCount / 2;
            if (Mathf.Abs(rotation - mouseRotation) < difference)
            {
                nearest = b;
                difference = Mathf.Abs(rotation - mouseRotation);
            }
        }
        SelectedSegment = nearest.name;
        float cursorRotation = -(mouseRotation - cursor.fillAmount * 360f / 2f) + 180f;
        cursorRotation = -(System.Convert.ToSingle(SelectedSegment) - cursor.fillAmount * 360f / 2f);
        cursor.transform.localRotation = Quaternion.Slerp(cursor.transform.localRotation, Quaternion.Euler(0, 0, cursorRotation), 0.14f);
    }
}
