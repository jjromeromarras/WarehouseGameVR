using System.Collections;
using TMPro;

public class inforesultcontroller : infotextbase
{
    

    public TextMeshProUGUI numresultado;
    public TextMeshProUGUI numtiempo;
    public TextMeshProUGUI numfallos;
    public TextMeshProUGUI numbonificacion;
       

    public void SetResult(int resultado, int tiempo, int fallos, int bonificacion)
    {
        numbonificacion.text = bonificacion.ToString();
        numfallos.text = $"- {fallos.ToString()}";
        numresultado.text = resultado.ToString();
        numtiempo.text = tiempo.ToString();
        GameManager.Instance.WriteLog($"[SetResult] - Score: {resultado} - tiempo: {tiempo} - fallos: {fallos} - bonificacion: {bonificacion}");
    }



    public IEnumerator SetMessageKey(float timeToWaitAfecterText, object[] arguments = null)
    {
        yield return SetMessage(textinfo.text, timeToWaitAfecterText);
    }

    
}
