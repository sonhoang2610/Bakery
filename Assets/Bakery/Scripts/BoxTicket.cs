using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EazyEngine.Tools;
using EazyEngine.Networking;
using System.Linq;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System.Data;
using CarlosAg.ExcelXmlWriter;
using UnityEngine.UI;
using UIWidgets;
using Crosstales.FB;


public class BoxTicket : BaseBox<ItemTicket, TicketStruct>
{
    [SerializeField]
    Text currentDateTime;
    [SerializeField]
    ResizableHeader attachMentProduct;
    [SerializeField]
    GameObject prefabNameProduct;
    [SerializeField]
    Text labelTime;
    [SerializeField]
    BoxBuyProduct boxTotal;
    protected string now;

    int currentfilterIndex = 0;
    TicketStruct[] cacheInfos;
    public static BoxTicket instance;
    public void selectIndexFilter(int pIndex)
    {
        currentfilterIndex = pIndex;
    }

    public void find(Text text)
    {
        if (string.IsNullOrEmpty(text.text))
        {
            return;
        }
        List<TicketStruct> finds = new List<TicketStruct>();
        for(int i = 0; i < cacheInfos.Length; ++i)
        {
            if(currentfilterIndex == 0 && cacheInfos[i].phone == text.text)
            {
                finds.Add(cacheInfos[i]);
            }
            if (currentfilterIndex == 1 && cacheInfos[i].name == text.text)
            {
                finds.Add(cacheInfos[i]);

            }
            if (currentfilterIndex == 2 && cacheInfos[i].nickname == text.text)
            {
                finds.Add(cacheInfos[i]);
            }
        }
        DataSource = finds.ToObservableList();

    }

    public static string convertToUnSign3(string s)
    {
        Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
        string temp = s.Normalize(NormalizationForm.FormD);
        return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
    }
    public virtual void deleteTicket(ItemTicket pTicketItem)
    {
        BoxDialog.Instance.showWarning(delegate {
            TicketStruct pTicket = pTicketItem._info;
            EzMessage pMsg = new EzMessage((ushort)ComonMsg.DeleteTicket);
            pMsg.addValue((byte)DeleteTicketMsg.CustomerID, pTicket.idCustomer);
            pMsg.addValue((byte)DeleteTicketMsg.DATETIME, now);
            pMsg.addValue((byte)DeleteTicketMsg.CANCEL, 1);
            ConnectionManager.Instance.Ws.Send(pMsg);
            reload();
            BoxDialog.Instance.gameObject.SetActive(false);
        });

        //  AttachMent.SendMessage("Reposition", AttachMent, SendMessageOptions.DontRequireReceiver);
    }

    public void deleteTicketNoReload(ItemTicket pTicketItem)
    {
        TicketStruct pTicket = pTicketItem._info;
        EzMessage pMsg = new EzMessage((ushort)ComonMsg.DeleteTicket);
        pMsg.addValue((byte)DeleteTicketMsg.CustomerID, pTicket.idCustomer);
        pMsg.addValue((byte)DeleteTicketMsg.DATETIME, now);
        pMsg.addValue((byte)DeleteTicketMsg.CANCEL, 1);
        ConnectionManager.Instance.Ws.Send(pMsg);
    }

    public virtual void fixItem(ItemTicket pTicketItem)
    {
        TicketStruct pTicket = pTicketItem._info;
        BoxBuy.Instance.gameObject.SetActive(true);
        DateTime time = DateTime.ParseExact(now, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
        CustomerInfo pInfo = new CustomerInfo();
        pInfo.id = int.Parse(pTicket.idCustomer);
        pInfo.profiles = new CustomerProfile[1];
        pInfo.profiles[0] = new CustomerProfile(pTicket.name, pTicket.phone, pTicket.adress, pTicket.note, "");
       
        BoxBuy.Instance.setInfoFix(pInfo, time, pTicket.tikets,delegate { deleteTicketNoReload(pTicketItem); });
    }

    public void addDay()
    {
        DateTime time = DateTime.ParseExact(now, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
        time = time.AddDays(1);
        now = time.ToString("dd/MM/yyyy");
        updateTime();
    }

    public void previousDay()
    {
        DateTime time = DateTime.ParseExact(now, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
        time = time.AddDays(-1);
        now = time.ToString("dd/MM/yyyy");
        updateTime();
    }

    public void setTime(string pTime)
    {
        now = pTime;
        updateTime();
    }

    public void updateTime()
    {
        currentDateTime.text = now;
        pgrogressLoadTicket(now);
        loadProductScehdule(now);
    }
    bool isInit = false;
    protected override void OnEnable()
    {
        base.OnEnable();
        if (isInit)
        {
            DateTime time = DateTime.Now;
            currentDateTime.text = time.ToString("dd/MM/yyyy");
            now = time.ToString("dd/MM/yyyy");
            pgrogressLoadTicket(time.ToString("dd/MM/yyyy"));
            loadProductScehdule(now);
        }

    }

    public void reload()
    {
        pgrogressLoadTicket(now);
    }

    public void onloadBoxInTime(int pIndex,ListViewItem pItem)
    {
        now = ((ItemCalendar)pItem)._info.time.ToString("dd/MM/yyyy");
        loadProductScehdule(((ItemCalendar)pItem)._info.time.ToString("dd/MM/yyyy"));
        pgrogressLoadTicket(((ItemCalendar)pItem)._info.time.ToString("dd/MM/yyyy"));
    }

    public override void Start()
    {
        base.Start();
        instance = this;

    }
    private void LateUpdate()
    {
        if (!isInit)
        {
            for (int i = 0; i < ProductConfig.nameProduct.Count; ++i)
            {
                var pCelll = Instantiate<GameObject>(prefabNameProduct,prefabNameProduct.transform.parent);
                pCelll.GetComponent<LayoutElement>().minWidth = 100;
                //DestroyImmediate(pCelll.GetComponent<ResizableHeaderDragCell>());
                //DestroyImmediate(pCelll.GetComponent<ResizableHeaderCell>());
                var pCell = Utilites.GetOrAddComponent<ResizableHeaderDragCell>(pCelll);
                pCell.Position = -1;
                attachMentProduct.AddCell(pCelll);
                pCelll.GetComponentInChildren<Text>().text = ProductConfig.nameProduct.ElementAt(i).Value.nameProduct;
            }
            DateTime time = DateTime.Now;
            currentDateTime.text = time.ToString("dd/MM/yyyy");
            now = time.ToString("dd/MM/yyyy");
            pgrogressLoadTicket(time.ToString("dd/MM/yyyy"));
            isInit = true;
        
        }
    }
    public virtual void pgrogressLoadTicket(string pTime)
    {
        labelTime.text = pTime;
        EzMessage pMsg = new EzMessage((ushort)ComonMsg.Loadticket);
        pMsg.addValue((byte)Loadticket.DATETIME, pTime);
        ConnectionManager.Instance.Ws.Send(pMsg);
        loading.gameObject.SetActive(true);
    }

    string[,] _excelData;
    int col = 8;
    int row = 0;
    protected override void onExecuteMessage(EzMessage msg)
    {
        if (msg.Header == (ushort)ComonMsg.LoadticketAck)
        {
            loading.gameObject.SetActive(false);
            EzTable table = (EzTable)msg.getValue((byte)LoadCustomerAcKParam.TABLE);
            //  object[,] pData = new object
            Dictionary<string, TicketStruct> dict = new Dictionary<string, TicketStruct>();
            List<TicketStruct> tickets = new List<TicketStruct>();
            for (int i = 0; i < table.row; ++i)
            {
                if (!dict.ContainsKey(table._datas[0, i].ToString()))
                {
                    dict.Add(table._datas[0, i].ToString(), new TicketStruct(table._datas[2, i].ToString(), table._datas[3, i].ToString(), table._datas[4, i].ToString(), table._datas[5, i].ToString(), table._datas[7, i].ToString(), new Pos[] { new Pos((int)table._datas[6, i], (int)table._datas[1, i]) }, table._datas[0, i].ToString(), table._datas[8, i].ToString()));
                    TicketStruct ticket = dict[table._datas[0, i].ToString()];
                    Pos[] pTickett = new Pos[ProductConfig.nameProduct.Keys.Count];
                    for(int k = 0; k  < ProductConfig.nameProduct.Keys.Count; ++k)
                    {
                        pTickett[k].x = int.Parse( ProductConfig.nameProduct.Keys.ElementAt(k));
                        pTickett[k].y = 0;
                        if (pTickett[k].x == (int)table._datas[6, i])
                        {
                            pTickett[k].y = (int)table._datas[1, i];
                        }
                    }
                    ticket.tikets = pTickett;
                }
                else
                {
                    TicketStruct ticket = dict[table._datas[0, i].ToString()];
                    int index = -1;
                    for (int j = 0; j < ticket.tikets.Length; ++j)
                    {
                        if (ticket.tikets[j].x == (int)table._datas[6, i])
                        {
                            index = j;

                        }
                    }
                    if (index >= 0)
                    {
                        Pos pos = ticket.tikets[index];
                        pos.y += (int)table._datas[1, i];
                        ticket.tikets[index] = pos;
                    }
                    else
                    {
                        Pos[] poss = ticket.tikets;
                        int pLenth = poss.Length;
                        System.Array.Resize(ref poss, poss.Length + 1);
                        poss[pLenth] = new Pos((int)table._datas[6, i], (int)table._datas[1, i]);
                        ticket.tikets = poss;
                    }
                    dict[table._datas[0, i].ToString()] = ticket;
                }
                // pDatas[i] = new TicketStruct(table._datas[2,i], table._datas[3, i], table._datas[4, i], table._datas[5, i],)
            }
            for (int i = 0; i < dict.Keys.Count; ++i)
            {
                tickets.Add(dict[dict.Keys.ElementAt(i)]);
            }
            List<string> title = new List<string>() { "STT", "Tên khách hàng", "Biệt Hiệu", "Số điện thoại", "Địa chỉ", "Lưu ý" };
            for (int i = 0; i < ProductConfig.nameProduct.Count; ++i)
            {
                title.Add(ProductConfig.nameProduct.ElementAt(i).Value.nameProduct);
            }
            title.Add("Tổng tiền");
            title.Add("Tien Ship");
            totalCol = title.Count;
            _excelData = new string[title.Count, tickets.Count + 1];
            for (int i = 0; i < title.Count; ++i)
            {
                _excelData[i, 0] = title[i];
            }
            List<ProductStatusInfo> listProducts = new List<ProductStatusInfo>();
            for (int i = 0; i < tickets.Count; ++i)
            {
                TicketStruct pInfo = tickets[i];
                string[] pStr = new string[ProductConfig.nameProduct.Count];
                int total = 0;
                for(int g = 0; g < pStr.Length; ++g )
                {
                    pStr[g] = "0";
                }
                for (int j = 0; j < pInfo.tikets.Length; ++j)
                {
                    pStr[j] = pInfo.tikets[j].y.ToString();
                    total += ProductConfig.nameProduct[pInfo.tikets[j].x.ToString()].price * pInfo.tikets[j].y;
                    int index = -1;
                    for(int k = 0; k < listProducts.Count; ++k)
                    {
                        if(listProducts[k].productID == pInfo.tikets[j].x.ToString())
                        {
                            index = k;
                        }
                    }
                    ProductStatusInfo pInfoss = null;
                    if (index < 0)
                    {
                       pInfoss = new ProductStatusInfo();
                        pInfoss.productID = pInfo.tikets[j].x.ToString();
                        pInfoss.quantityUsed = pInfo.tikets[j].y;
                        pInfoss.quantitySchedule = pInfo.tikets[j].y;
                        listProducts.Add(pInfoss);
                    }
                    else
                    {
                        pInfoss = listProducts[index];
                        pInfoss.quantityUsed += pInfo.tikets[j].y;
                        pInfoss.quantitySchedule += pInfo.tikets[j].y;
                    }
                }
                
                string pPhone = pInfo.phone;
                string pAdress = pInfo.adress;
                pAdress = convertToUnSign3(pAdress);
                int price = 0;
                for (int j = 0; j < AdressConfig.price.Keys.Count; ++j)
                {
                    if (pAdress.Contains(AdressConfig.price.Keys.ElementAt(j)))
                    {
                        price = AdressConfig.price[AdressConfig.price.Keys.ElementAt(j)];
                    }
                }
                List<string> content = new List<string>() { i.ToString(), pInfo.name, pInfo.nickname, pPhone, pInfo.adress, pInfo.note };
                for (int j = 0; j < ProductConfig.nameProduct.Count; ++j)
                {
                    content.Add(pStr[j]);
                }
                content.Add(total.ToString());
                content.Add(price.ToString());
                for (int j = 0; j < content.Count; j++)
                {
                    _excelData[j, i + 1] = content[j];
                }
            }
      
            row = tickets.Count + 1;
            //executeInfos(tickets.ToArray());
            DataSource = tickets.ToObservableList();
            cacheInfos = tickets.ToArray();
    
            boxTotal.DataSource = listProducts.ToObservableList();
         //   attachMentProduct.GetComponent<ResizableHeader>().Resize();
         attachMentProduct.GetComponent<ResizableHeader>().Invoke("Resize",0.5f);
        }
        if (msg.Header == (ushort)ComonMsg.LoadProductScehduleACK)
        {
            //EzTable pTable = (EzTable)msg.getValue((byte)LoadCustomerAcKParam.TABLE);
            //ProductStatusInfo[] infos = new ProductStatusInfo[pTable.row];
            //int schedule = 0, ticket = 0;
            //for (int i = 0; i < infos.Length; ++i)
            //{
            //    schedule += (int)pTable._datas[1, i];
            //    ticket += (int)pTable._datas[2, i];
            //}
            //scheduleLabel.text = string.Format("[FF9C00]Đã đặt:[-] [FFF700]{0}[-]", ticket);
            //storageLabel.text = string.Format("[FF9C00]Hàng tồn:[-] [FFF700]{0}[-]", (schedule - ticket) < 0 ? 0 : (schedule - ticket));
        }
    }
    public void loadProductScehdule(string pDateTime)
    {
        EzMessage pMSg = new EzMessage((byte)ComonMsg.LoadProductScehdule);
        pMSg.addValue((byte)LoadProductSchedule.DATETIME, pDateTime);
        ConnectionManager.Instance.Ws.Send(pMSg);
    }

    //public override void executeInfos(TicketStruct[] pInfos)
    //{
    //    base.executeInfos(pInfos);
    //    setEventBtn();
    //}
    protected override void SetData(ItemTicket component, TicketStruct item)
    {
        base.SetData(component, item);
        component.onRed = deleteTicket;
        component.onGreen = fixItem;
    }

    //public virtual void setEventBtn()
    //{
    //    for (int i = 0; i < items.Count; ++i)
    //    {
    //        if (items[i].gameObject.activeSelf)
    //        {
    //            items[i].onRed = deleteTicket;
    //            items[i].onGreen = fixItem;
    //        }
    //    }
    //}
    int totalCol = 0;
    [ContextMenu("export")]
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
