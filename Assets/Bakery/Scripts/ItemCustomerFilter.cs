using CarlosAg.ExcelXmlWriter;
using Crosstales.FB;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;


public class HistoryTickets
{
    public string name, phone,datetime;
    public int[] quantityProduct;
}
public class ItemCustomerFilter : BaseItem<CustomerInfo>, IResizableItem
{
    [SerializeField]
    Text  name_label, phone;
    [SerializeField]
    LayoutGroup attachMentProduct;
    [SerializeField]
    GameObject prefabNameProduct;
    [SerializeField]
    Graphic[] foreground;
    [SerializeField]
    [HideInInspector]
    List<GameObject> listLabelProduct = new List<GameObject>();
    List<Graphic> listFore = new List<Graphic>();
    protected override void Awake()
    {
        listFore.AddRange(foreground);
        base.Awake();

    }
    int totalCol;
    protected override void Start()
    {
        base.Start();
   
        if (listLabelProduct.Count == 0)
        {
            for (int i = 0; i < ProductConfig.nameProduct.Count; ++i)
            {
                var pCell = Instantiate<GameObject>(prefabNameProduct, attachMentProduct.transform);
                var pLAbel = pCell.GetComponent<Text>();
                if (_info != null)
                {
                    pLAbel.text = (_info != null && i < _info.tickets.Count) ? _info.tickets[i].y.ToString() : "0";
                }
                // pLAbel.name = i.ToString("D3");
                listLabelProduct.Add(pCell);
                listFore.Add(pLAbel);
            }
        }
    }

    public override void SetData(CustomerInfo pInfo)
    {
        totalCol = ProductConfig.nameProduct.Keys.Count + 4;
        base.SetData(pInfo);
        name_label.text = pInfo.nickname;
        string pPhone = pInfo.phone;
        if (!string.IsNullOrEmpty(pPhone))
        {
            pPhone = pPhone.Insert(7, ".");
            pPhone = pPhone.Insert(4, ".");
            phone.text = pPhone;
        }
        for(int i = 0; i < listLabelProduct.Count; ++i)
        {
            listLabelProduct[i].GetComponentInChildren<Text>().text = pInfo.tickets[i].y.ToString();
        }
        for(int j = pInfo.detailHistory.Count -1; j >=0 ; --j)
        {
            int pTotal = 0;
            for(int i  = 0; i < pInfo.detailHistory[j].quantityProduct.Length; ++i)
            {
                pTotal += pInfo.detailHistory[j].quantityProduct[i];
            }
            if(pTotal == 0)
            {
                pInfo.detailHistory.RemoveAt(j);
            }
        }
        _excelData = new string[totalCol, 1 + pInfo.detailHistory.Count];
        List<string> pCol = new List<string>() { "Tên","Phone","Ngày Đặt" };
        for(int i = 0; i< ProductConfig.nameProduct.Count; ++i)
        {
            pCol.Add(ProductConfig.nameProduct[ProductConfig.nameProduct.Keys.ElementAt(i)].nameProduct);
        }
        pCol.Add("Thành Tiền");
        row = pInfo.detailHistory.Count + 1;
        for (int i = 0; i < totalCol; ++i)
        {
            _excelData[i, 0] = pCol[i];
           for (int j  = 0; j < pInfo.detailHistory.Count; ++j)
            {
                if(i == 0)
                {
                    _excelData[i, j + 1] = pInfo.detailHistory[j].name;
                }else if(i == 1)
                {
                    _excelData[i, j + 1] = pInfo.detailHistory[j].phone;
                }
                else if (i == 2)
                {
                    _excelData[i, j + 1] = pInfo.detailHistory[j].datetime;
                }
                else if(i < totalCol-1)
                {
                    _excelData[i, j + 1] = pInfo.detailHistory[j].quantityProduct[i-3].ToString();
                }
                else
                {
                    int totalprice = 0;
                    for(int k = 0; k < pInfo.detailHistory[j].quantityProduct.Length; ++k)
                    {
                        int price = ProductConfig.nameProduct[ProductConfig.nameProduct.Keys.ElementAt(k)].price;
                        totalprice += pInfo.detailHistory[j].quantityProduct[k] * price;
                    }
                    _excelData[i, j + 1] = totalprice.ToString();
                }
            }
        }
    }

    public void detail()
    {
        BoxProfile.instance.gameObject.SetActive(true);
        BoxProfile.instance.showBoxWithID(_info.id);
    }
    public GameObject[] ObjectsToResize
    {
        get
        {
            List<GameObject> pObjects = new List<GameObject>() { name_label.gameObject, phone.gameObject };
            pObjects.AddRange(listLabelProduct.ToArray());
            return pObjects.ToArray();
        }
    }
    public override Graphic[] GraphicsForeground
    {
        get
        {
            return listFore.ToArray();
        }
    }
    string[,] _excelData;
    int row = 0;
    public void export()
    {
        string extensions = "xls";

        string path = Application.persistentDataPath + "Myfile" + "." + extensions;
#if UNITY_EDITOR || UNITY_STANDALONE
        path = FileBrowser.SaveFile("Save File", "", "Myfile", extensions);
#endif

        StartCoroutine(CrearArchivoCSV(path));
    }

    IEnumerator CrearArchivoCSV(string ruta)
    {
        //El archivo existe? lo BORRAMOS
        if (File.Exists(ruta))
        {
            File.Delete(ruta);
        }
        Workbook book = new Workbook();
        Worksheet sheet = book.Worksheets.Add("Sheet1");
        for (int i = 0; i < row; ++i)
        {
            WorksheetRow pRow = sheet.Table.Rows.Add();
            for (int j = 0; j < totalCol; j++)
            {
                int pValue = 0;
                if (int.TryParse(_excelData[j, i], out pValue))
                {
                    pRow.Cells.Add(new WorksheetCell(pValue.ToString(), DataType.Number));
                }
                else
                {
                    pRow.Cells.Add(_excelData[j, i].ToString());
                }

            }
        }

        book.Save(ruta);

        yield return new WaitForSeconds(0.5f);//Esperamos para estar seguros que escriba el archivo

        //Abrimos archivo recien creado
        Application.OpenURL(ruta);

    }
}
