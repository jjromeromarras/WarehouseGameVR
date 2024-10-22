using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class GamepadCursor : MonoBehaviour
{
    public RectTransform cursorTransform; // El rect transform del puntero (cursor)
    public float cursorSpeed = 1000f;     // Velocidad del puntero
    public Canvas canvas;                 // El canvas que contiene el puntero

    private Vector2 cursorPosition;
    private PointerEventData pointerEventData;
    public EventSystem eventSystem;       // El sistema de eventos de Unity

    void Start()
    {
        // Inicializa el cursor en el centro de la pantalla
        cursorPosition = new Vector2(Screen.width / 2, Screen.height / 2);

        // Inicializar el PointerEventData
        pointerEventData = new PointerEventData(eventSystem);
    }

    void Update()
    {
        // Obtener los valores del joystick izquierdo
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        // Mover el cursor
        Vector2 move = new Vector2(moveX, moveY) * cursorSpeed * Time.deltaTime;
        cursorPosition += move;

        // Limitar la posici�n del cursor para que no salga de la pantalla
        cursorPosition.x = Mathf.Clamp(cursorPosition.x, 0, Screen.width);
        cursorPosition.y = Mathf.Clamp(cursorPosition.y, 0, Screen.height);

        // Actualizar la posici�n del cursor en pantalla
        cursorTransform.position = cursorPosition;

        // Actualizar la posici�n del cursor en el sistema de eventos
        pointerEventData.position = cursorPosition;

        // Verificar si se presiona el bot�n A para simular clics
        if (Input.GetKeyDown(KeyCode.Joystick1Button0)) // Asumiendo que "Submit" est� mapeado al bot�n A
        {
            // Crear una lista para guardar los objetos que est�n debajo del cursor
            var raycastResults = new System.Collections.Generic.List<RaycastResult>();
            eventSystem.RaycastAll(pointerEventData, raycastResults);

            // Si el cursor est� sobre un bot�n, ejecutar el clic
            foreach (RaycastResult result in raycastResults)
            {
                // Busca el componente Button en el objeto o en sus padres
                Button button = result.gameObject.GetComponentInParent<Button>();

                if (button != null)
                {
                    // Invoca el evento onClick del bot�n si fue encontrado
                    button.onClick.Invoke();          
                    return; // Sale despu�s de invocar el bot�n
                }
                else
                {
                    // Buscamos
                    Toggle toggle = result.gameObject.GetComponentInParent<Toggle>();
                    if (toggle != null) 
                    {
                        toggle.isOn = !toggle.isOn;
                        toggle.onValueChanged.Invoke(toggle.isOn);
                        return;
                    }
                    else
                    {
                        TMP_Dropdown dropdown = result.gameObject.GetComponent<TMP_Dropdown>();

                        if (dropdown != null)
                        {
                            // Cambia el �ndice de selecci�n del dropdown
                            int newIndex = (dropdown.value + 1) % dropdown.options.Count; // Cambia al siguiente elemento
                            dropdown.value = newIndex;

                            // Invoca el evento onValueChanged
                            dropdown.onValueChanged.Invoke(newIndex);

                            Debug.Log("Dropdown actualizado: " + dropdown.name + " | Nuevo valor seleccionado: " + dropdown.options[newIndex].text);
                            return; // Salir despu�s de seleccionar el dropdown
                        }
                    }
                }
            }
        }
    }
}
