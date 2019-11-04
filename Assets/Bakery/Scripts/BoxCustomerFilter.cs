using System.Collections;
using System.Collections.Generic;
using EazyEngine.Networking;
using UnityEngine;
using UnityEngine.UI;
using UIWidgets;
using System.Linq;
using CarlosAg.ExcelXmlWriter;
using System.IO;
using Crosstales.FB;

public class BoxCustomerFilter : BaseBox<ItemCustomerFilter,CustomerInfo> {
    [HideInInspector]
    public CustomerInfo[] _infos;
    [SerializeField]
    ResizableHeader attachMentProduct;
    [SerializeField]
    GameObject prefabNameProduct;
    [SerializeField]
    BoxBuyProduct boxTotal;
    protected override void onExecuteMessage(EzMessage msg)
    {
         if(msg.Header == (ushort)ComonMsg.AnalizeCustomerACK)
        {
            executeFilterCustomer(msg);
        }
    }

    public override void updatePage()
    {
        //List<CustomStruct> pInfoPages = new List<CustomStruct>();
        //int pLimit = currentPage * 10 + 10;
        //for (int i = currentPage*10; i < pLimit; ++i)
        //{
        //    if (i < _infos.Length ){
        //        pInfoPages.Add(_infos[i]);
        //    }
        //}
        //executeInfos(pInfoPages.ToArray());
        progressLoadCustomer(cacheMethod);
    }
    bool pActive = false;
    public void progressLoadCustomerInput(Text pMethod)
    {
        pActive = true;
        progressLoadCustomer(pMethod.text);
    }
    string planExportforID = "";
    public void detailExport(object pObject)
    {
        CustomerInfo pInfo =(CustomerInfo) pObject;
        pActive = true;
        planExportforID = pInfo.id.ToString();
        progressLoadCustomer(cacheMethod, true, pInfo.id.ToString());
    }
    string cacheMethod;
    public void progressLoadCustomer(string pMethod,bool pFull = false,string pCustomerID= "")
    {
        cacheMethod = pMethod;
        if (string.IsNullOrEmpty(cacheMethod)) return;
        EzMessage pMsg = new EzMessage((ushort)ComonMsg.AnalizeCustomer);
        pMsg.addValue((byte)AnalizeCustomer.Method, pMethod);
        pMsg.addValue((byte)AnalizeCustomer.Page, pFull ? -1 : currentPage);
        pMsg.addValue((byte)AnalizeCustomer.SpecifiedID, pCustomerID);
        ConnectionManager.Instance.Ws.Send(pMsg);
        loading.gameObject.SetActive(true);
    }
    string[,] _excelData;
    int row = 0;
    public void executeFilterCustomer(EzMessage msg)
    {

        EzTable tableData = (EzTable)msg.getValue((byte)LoadCustomerFilterAcKParam.TABLE);
        string  pTotal = msg.getString((byte)LoadCustomerFilterAcKParam.TOTAL);
        int pAge = msg.getInt((byte)LoadCustomerFilterAcKParam.Page);
        int pFull = msg.getInt((byte)LoadCustomerFilterAcKParam.FULL);
        string[] pTotals = pTotal.Split(',');
        List<CustomerInfo> pInfos = new List<CustomerInfo>();
        List<HistoryTickets> pTicketHistory = new List<HistoryTickets>();
        object[,] data = tableData._datas;
        List<ProductStatusInfo> listProducts = new List<ProductStatusInfo>() ;
        for(int i = 0;  i < ProductConfig.nameProduct.Count; ++i)
        {
            var pProduct = new ProductStatusInfo(ProductConfig.nameProduct.Keys.ElementAt(i), 0, 0);
            listProducts.Add(pProduct);
            for(int j  = 0; j < pTotals.Length/2; j++)
            {
                if(pProduct.productID == pTotals[j * 2])
                {
                    pProduct.quantitySchedule = int.Parse( pTotals[j * 2 + 1]);
                    pProduct.quantityUsed = int.Parse(pTotals[j * 2 + 1]);
                }
            }
        }
        for (int i = 0; i < tableData.row; ++i)
        {
            CustomerInfo pCustom = null ;
            int pID = (int)data[0, i];
            for(int j  = 0; j < pInfos.Count; ++j)
            {
                if(pInfos[j].id == pID)
                {
                    pCustom = pInfos[j];
                    break;
                }
            }
            if (pCustom == null)
            {
                pCustom = new CustomerInfo();
                pInfos.Add(pCustom);
            }
            if (pCustom.tickets .Count == 0)
            {
                for (int j = 0; j < ProductConfig.nameProduct.Count; ++j)
                {
                    pCustom.tickets.Add(new Pos(int.Parse(ProductConfig.nameProduct.Keys.ElementAt(j).ToString()), 0));
                }
            }
            for (int j = 0; j < pCustom.tickets.Count; ++j)
            {
                if(pCustom.tickets[j].x == (int)data[5, i])
                {
                    Pos pTicket = pCustom.tickets[j];
                    pTicket. y += (int)data[4, i];
                    pCustom.tickets[j] = pTicket;
                }
            }
            pCustom.nickname = data[1, i].ToString();
            pCustom.id = (int)data[0, i];
            pCustom.number_bought = (int)data[4, i];
            pCustom.buy = (int)data[3, i];
            pCustom.profiles = new CustomerProfile[1];
            pCustom.profiles[0] = new CustomerProfile("", data[2, i].ToString(), data[7, i].ToString(), "","");
            int[] product = new int[ProductConfig.nameProduct.Keys.Count];
            for(int g = 0; g < product.Length; ++g)
            {
                if(g == (int)data[5, i])
                {
                    product[g] = (int)data[4, i];
                }
                else
                {
                    product[g] = 0;
                }
            }
            pCustom.detailHistory.Add(new HistoryTickets() { name = pCustom.nickname, datetime = data[6, i].ToString(), phone = data[2, i].ToString(), quantityProduct = product });
        }
        DataSource = pInfos.ToObservableList();
        boxTotal.DataSource = listProducts.ToObservableList();
        setLimit(pAge);
        loading.gameObject.SetActive(false);
        row = pInfos.Count + 1;
        _excelData = new string[3+ ProductConfig.nameProduct.Keys.Count, row];
        totalCol = 3 + ProductConfig.nameProduct.Keys.Count;
        List<string> titles = new List<string> { "Tên khách", "Phone","adress" };
        for (int i = 0; i < ProductConfig.nameProduct.Count; ++i)
        {
            titles.Add(ProductConfig.nameProduct.ElementAt(i).Value.nameProduct);
        }
        for (int j = 0; j < row; ++j)
        {
            for (int i = 0; i < 3 + ProductConfig.nameProduct.Keys.Count; ++i)
            {
                if (j == 0)
                {
                    _excelData[i, 0] = titles[i];
                }
                else
                {
                    if( i == 0)
                    {
                        _excelData[i, j] = pInfos[j - 1].nickname;
                    }else
                    if(i == 1)
                    {
                        _excelData[i, j] = pInfos[j - 1].phone;
                    }else if(i == 2)
                    {
                        _excelData[i, j] = pInfos[j - 1].adress;
                    }
                    else
                    {
                        _excelData[i, j] = pInfos[j - 1].tickets[i-3].y.ToString();
                    }
                
                }
                
            }

        }
        if(pFull == 1 && !pActive)
        {
            export();
        }
        pActive = false;
        planExportforID = "";
    }

    protected override void SetData(ItemCustomerFilter component, CustomerInfo item)
    {
        base.SetData(component, item);
        if(item.id.ToString() == planExportforID)
        {
            planExportforID = "";
            component.export();
        }
    }
    bool isInit = false;
    private void LateUpdate()
    {
        if (!isInit)
        {
            currentPage = -1;
            for (int i = 0; i < ProductConfig.nameProduct.Count; ++i)
            {
                var pCelll = Instantiate<GameObject>(prefabNameProduct, prefabNameProduct.transform.parent);
                var pCell = Utilites.GetOrAddComponent<ResizableHeaderDragCell>(pCelll);
                pCell.Position = -1;
                attachMentProduct.AddCell(pCelll);
                pCelll.GetComponentInChildren<Text>().text = ProductConfig.nameProduct.ElementAt(i).Value.nameProduct;
            }
            isInit = true;
        }
    }
    int totalCol = 0;
    [ContextMenu("export")]
    public void startExport()
    {
        progressLoadCustomer(cacheMethod,true);
    }


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
