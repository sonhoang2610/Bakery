using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UIWidgets;

public class TicketStruct
{
    public string idCustomer;
    public string name, phone, facebook, adress, note,nickname;
    public Pos[] tikets;

    public TicketStruct(string name,string phone,string facebook,string adress,string note,Pos[] tickets,string pIDCus,string pNickName )
    {
        this.name = name;
        this.phone = phone;
        this.facebook = facebook;
        this.adress = adress;
        this.note = note;
        this.tikets = tickets;
        idCustomer = pIDCus;
        nickname = pNickName;
    }

    public TicketStruct()
    {

    }
}
public class ItemTicket : BaseItem<TicketStruct>, IResizableItem
{
    public Text nameItem, phone, adress, note;
    [SerializeField]
    LayoutGroup attachMentProduct;
    [SerializeField]
    GameObject prefabNameProduct;
    [SerializeField]
    Graphic[] foreground;
    public delegate void eventOnTicket(ItemTicket a);
    public eventOnTicket onRed, onGreen;
    [SerializeField]
    [HideInInspector]
    List<GameObject> listLabelProduct = new List<GameObject>();
    public void onRedBtn()
    {
        if (onRed != null)
        {
            onRed(this);
        }
    }

    public void onGreenBtn()
    {
        if (onGreen != null)
        {
            onGreen(this);
        }
    }

    public void openUrl()
    {
#if !UNITY_EDITOR && !UNITY_STANDALONE
        Application.OpenURL("tel://" + _info.phone);
#endif
    }
    protected override void Awake()
    {
        listFore.AddRange(foreground);
        base.Awake();
    
    }
    
    List<Graphic> listFore = new List<Graphic>();
    public override Graphic[] GraphicsForeground
    {
        get
        {
            return listFore.ToArray();
        }
    }

    public GameObject[] ObjectsToResize
    {
        get
        {
            List<GameObject> pObjects = new List<GameObject>() { nameItem.gameObject, phone.gameObject, adress.gameObject, note.gameObject };
            pObjects.AddRange(listLabelProduct.ToArray());
            return pObjects.ToArray();
        }
    }

    //private override void Awake()
    //{
    //    for (int i = 0; i < ProductConfig.nameProduct.Count; ++i)
    //    {
    //        var pLAbel = Instantiate<UILabel>(prefabNameProduct, attachMentProduct.transform);

    //        pLAbel.text = ProductConfig.nameProduct.ElementAt(i).Value.nameProduct;
    //        pLAbel.depth = attachMentProduct.GetComponentInParent<UIWidget>().depth + 1;
    //        pLAbel.name = i.ToString("D3");
    //        listLabelProduct.Add(pLAbel);
    //    }
    //}
    protected override void Start()
    {
        base.Start();
        if (listLabelProduct.Count == 0)
        {
            for (int i = 0; i < ProductConfig.nameProduct.Count; ++i)
            {
                var pCell = Instantiate<GameObject>(prefabNameProduct, attachMentProduct.transform);
                var pLAbel = pCell.GetComponent<Text>();
                pLAbel.text = _info != null  ? _info.tikets[i].y.ToString() : "0";
               // pLAbel.name = i.ToString("D3");
                listLabelProduct.Add(pCell);
                listFore.Add(pLAbel);
            }
        }
    }
    public override void SetData(TicketStruct pInfo)
    {
        base.SetData(pInfo);
        nameItem.text = pInfo.nickname;
        string pPhone = pInfo.phone;
        pPhone = pPhone.Insert(7, ".");
        pPhone = pPhone.Insert(4, ".");
        phone.text = pPhone;
        adress.text = pInfo.adress;
        note.text = pInfo.note;
        string pStr = "";
        for (int i = 0; i < listLabelProduct.Count; ++i)
        {
            int index = -1;
            for(int j = 0; j < pInfo.tikets.Length; ++j)
            {
               if(pInfo.tikets[j].x.ToString() == ProductConfig.nameProduct.ElementAt(i).Key)
                {
                    index = j;
                    break;
                }
            }
            if(index != -1)
            {
                listLabelProduct[i].GetComponent<Text>().text = pInfo.tikets[index].y.ToString();
            }
            else
            {
                listLabelProduct[i].GetComponent<Text>().text = "0";
            }
        }
    }
}
