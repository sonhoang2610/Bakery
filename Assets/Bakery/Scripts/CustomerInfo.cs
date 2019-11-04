using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class CustomerInfo
{
    public int id, score, buy, cancel;
    public int number_bought;
    public List<Pos> tickets = new List<Pos>();
    public string facebookID,nickname,avaLink,description;
    public CustomerProfile[] profiles;
    public List<HistoryTickets> detailHistory = new List<HistoryTickets>();
    public string name
    {
        get
        {
            if(profiles != null && profiles.Length > 0)
            {
                return profiles[0].name;
            }
            return "";
        }
    }
    public string phone
    {
        get
        {
            if (profiles != null && profiles.Length > 0)
            {
                return profiles[0].phone;
            }
            return "";
        }
    }
    public string adress
    {
        get
        {
            if (profiles != null && profiles.Length > 0)
            {
                return profiles[0].adress;
            }
            return "";
        }
        set
        {
            profiles[0].adress = value;
        }
    }
    public string note
    {
        get
        {
            if (profiles != null && profiles.Length > 0)
            {
                return profiles[0].note;
            }
            return "";
        }
    }
    public string email
    {
        get
        {
            if (profiles != null && profiles.Length > 0)
            {
                return profiles[0].email;
            }
            return "";
        }
    }
}

[System.Serializable]
public class CustomerProfile
{
    public string name, phone, adress, note, email;
    public int profileID;
    public CustomerProfile(string name,string phone,string adress,string note,string email,int profileID = 0)
    {
        this.name = name;
        this.phone = phone;
        this.adress = adress;
        this.note = note;
        this.email = email;
        this.profileID = profileID;
    }
}
