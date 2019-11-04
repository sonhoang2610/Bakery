

public enum ComonMsg
{
    LoadCustomerTable = 0x00,
    LoadCustomerTableACK,
    FilterCustomer,
    FilterCustomerACK,
    AnalizeCustomer,
    AnalizeCustomerACK,
    Loadticket,
    LoadticketAck,
    LoadticketUnAccept,
    LoadProduct,
    LoadProductAck,
    LoadProductScehdule,
    LoadProductScehduleACK,
    UpdateProductSchedule,
    InsertTicket,
    InsertTicketACK,
    DeleteTicket,
    AcceptTicket,
    LoadProfile,
    LoadProfileAck,
    UpdateProfile,
    LoadScheduleFrom,
    LoadScheduleFromACK,
    FilterCustomerNextGen,
    UpdateProfileNextGen,
    LoadProfileNextGen,
    LoadProfileNextGenACK,
}

public enum UpdateProfileRequestNexGen
{
    CustomerID = BaseAckMsg.BaseParam,
    name,
    phone,
    facebook,
    des,
    profileID,
    nametaken,
    adress,
    note
}

public enum UpdateProfileRequest
{
    CustomerID = BaseAckMsg.BaseParam,
    name,
    score,
    ava,
    facebook,
    des
}

public enum LoadProfileRequest
{
    CustomerID = BaseAckMsg.BaseParam
}

public enum LoadProfileResponse
{
    TABLE = BaseAckMsg.BaseParam
}

public enum DeleteTicketMsg
{
    DATETIME = BaseAckMsg.BaseParam,
    CustomerID,
    CANCEL
}



public enum ERROR_MSG
{
    ERROR_CODE = 100
}
public enum InsertTicketMsg
{
    DATETIME = BaseAckMsg.BaseParam,
    PRODUCT_IDS,
    PRODUCTS,
    PHONE,
    NAME,
    ADRESS,
    EMAIL,
    NOTE
}
public enum Loadticket
{
    DATETIME = BaseAckMsg.BaseParam
}

public enum UpdateScehduleMsg
{
    DATETIME = BaseAckMsg.BaseParam,
    PRODUCT_IDS,
    PRODUCTS,
}

public enum BaseAckMsg
{
    BaseParam = 0x00,
}

public enum LoadProductSchedule
{
    DATETIME = BaseAckMsg.BaseParam
}

public enum LoadCustomerAcKParam
{
    TABLE = BaseAckMsg.BaseParam,
    PAGE,
    TABLE_ADRESS
}

public enum LoadCustomerFilterAcKParam
{
    TABLE = BaseAckMsg.BaseParam,
    Page,
    TOTAL,
    FULL
}


public enum FilterCustomerParam
{
    Filter = BaseAckMsg.BaseParam,
    Value,
    Page
}
public enum AnalizeCustomer
{
    Method = BaseAckMsg.BaseParam,
    Page,
    SpecifiedID
}

public enum LoadScheduleFrom
{
    Time = BaseAckMsg.BaseParam,
}
public enum LoadScheduleFromACK
{
    TABLE = BaseAckMsg.BaseParam,
}
