using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIWidgets;
using EazyEngine.Networking;

public class BoxProfile : MonoBehaviour, IViewData<CustomerInfo>
{
    [SerializeField]
    InputField mainName, numberBuy, numberProductBuy, numberCancel, score, description, phone, faceBookLink;
    [SerializeField]
    ListView _listProfile;
    [SerializeField]
    InputField nameTaken, note, adress;
    [SerializeField]
    LoadUrlPic ava;

    public static BoxProfile instance;
    private void Awake()
    {
        instance = this;
    }

    protected CustomerInfo _info;
    protected int currentIndexProfile = 0;
    public void SetData(CustomerInfo item)
    {
        _info = item;
        // string[] pString = new string[item.profiles.Length];
        //for(int i = 0; i < item.profiles.Length; ++i)
        // {
        //     pString[i] = "Profile " + (i+1);
        // }
        // _listProfile.DataSource = pString.ToObservableList();
        mainName.text = item.nickname;
        numberBuy.text = item.buy.ToString();
        numberProductBuy.text = item.number_bought.ToString();
        numberCancel.text = item.cancel.ToString();
        //score.text = "0";
        description.text = item.description;
        phone.text = item.phone;
        faceBookLink.text = item.facebookID;
        onSelectProfile(0);
    }
    private void onExecuteMsg(IClient ws, EzMessage msg)
    {
        Dispatcher.Invoke(delegate
        {
            if (msg.Header == (ushort)ComonMsg.LoadProfileNextGenACK)
            {

                EzTable tableData = (EzTable)msg.getValue((byte)LoadCustomerAcKParam.TABLE);
                List<CustomerInfo> pInfos = new List<CustomerInfo>();
                object[,] data = tableData._datas;
                for (int i = 0; i < tableData.row; ++i)
                {
                    CustomerInfo pInfo = new CustomerInfo();
                    pInfo.id = (int)data[9, i];
                    pInfo.nickname = data[4, i].ToString();
                    pInfo.number_bought = (int)data[14, i];
                    pInfo.buy = (int)data[15, i];
                    pInfo.facebookID = data[5, i].ToString();
                    pInfo.avaLink = data[6, i].ToString();
                    pInfo.cancel = (int)data[2, i];
                    pInfo.score = (int)data[2, i];
                    pInfo.description = data[17, i].ToString();
                    pInfos.Add(pInfo);
                    List<CustomerProfile> listprofile = new List<CustomerProfile>();
                    if (pInfo.profiles != null)
                    {
                        listprofile.AddRange(pInfo.profiles);
                    }
                    listprofile.Add(new CustomerProfile(data[12, i].ToString(), data[10, i].ToString(), data[11, i].ToString(), data[1, i].ToString(), "", (int)data[16, i]));
                    pInfo.profiles = listprofile.ToArray();

                }
                if (pInfos.Count > 0)
                {
                    SetData(pInfos[0]);
                }
            }
        });
    }

    public void showBoxWithID(int pID)
    {
        EzMessage pMsg = new EzMessage((ushort)ComonMsg.LoadProfileNextGen);
        pMsg.addValue((byte)LoadProfileRequest.CustomerID, pID);
        ConnectionManager.Instance.Ws.Send(pMsg);
    }

    public void updateProfile()
    {
        EzMessage pMsg = new EzMessage((ushort)ComonMsg.UpdateProfileNextGen);
        pMsg.addValue((byte)UpdateProfileRequestNexGen.CustomerID, _info.id);
        pMsg.addValue((byte)UpdateProfileRequestNexGen.des, description.text);
        pMsg.addValue((byte)UpdateProfileRequestNexGen.facebook, faceBookLink.text);
        pMsg.addValue((byte)UpdateProfileRequestNexGen.name, mainName.text);
        pMsg.addValue((byte)UpdateProfileRequestNexGen.profileID, _info.profiles[0].profileID);
        pMsg.addValue((byte)UpdateProfileRequestNexGen.nametaken, nameTaken.text);
        pMsg.addValue((byte)UpdateProfileRequestNexGen.adress, adress.text);
        pMsg.addValue((byte)UpdateProfileRequestNexGen.note, note.text);
        pMsg.addValue((byte)UpdateProfileRequestNexGen.phone, phone.text);
        ConnectionManager.Instance.Ws.Send(pMsg);
        gameObject.SetActive(false);
        ListViewCustomer.instance.updatePage();
    }
    private void OnEnable()
    {
        EzClient.OnMessage += onExecuteMsg;
    }

    private void OnDisable()
    {
        EzClient.OnMessage -= onExecuteMsg;
    }
    public void onSelectProfile(int pIndex)
    {
        currentIndexProfile = pIndex;
        CustomerProfile profile = _info.profiles[pIndex];
        nameTaken.text = profile.name;
        note.text = profile.note;
        adress.text = profile.adress;

    }

    public void ticket()
    {
        gameObject.SetActive(false);
        BoxBuy.instance.gameObject.SetActive(true);
        BoxBuy.instance.setInfo(_info);
    }
}
