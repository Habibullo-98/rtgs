using System.Security.Cryptography.Xml;
using RTGS.Models.Abs;
using RTGS.Models.Controller;
using RTGS.Models.DB;
using RTGS.Models.ISO20022;
using RTGS.Models.ISO20022.AppHeader;
using RTGS.Models.ISO20022.CustomerCreditTransfer;
using RTGS.Models.ISO20022.InstitutionCreditTransfer;
using RTGS.Models.ISO20022.DebitCreditNotification;
using RTGS.Models.ISO20022.Statement;
using RTGS.Services.DB;
using AccountIdentification4Choice = RTGS.Models.ISO20022.CustomerCreditTransfer.AccountIdentification4Choice;
using ActiveCurrencyAndAmount = RTGS.Models.ISO20022.CustomerCreditTransfer.ActiveCurrencyAndAmount;
using BranchAndFinancialInstitutionIdentification6 = RTGS.Models.ISO20022.CustomerCreditTransfer.BranchAndFinancialInstitutionIdentification6;
using CashAccount38 = RTGS.Models.ISO20022.CustomerCreditTransfer.CashAccount38;
using ChargeBearerType1Code = RTGS.Models.ISO20022.CustomerCreditTransfer.ChargeBearerType1Code;
using ClearingSystemMemberIdentification2 = RTGS.Models.ISO20022.AppHeader.ClearingSystemMemberIdentification2;
using FinancialInstitutionIdentification18 = RTGS.Models.ISO20022.CustomerCreditTransfer.FinancialInstitutionIdentification18;
using GenericAccountIdentification1 = RTGS.Models.ISO20022.CustomerCreditTransfer.GenericAccountIdentification1;
using GenericOrganisationIdentification1 = RTGS.Models.ISO20022.CustomerCreditTransfer.GenericOrganisationIdentification1;
using ItemChoiceType6 = RTGS.Models.ISO20022.CustomerCreditTransfer.ItemChoiceType6;
using ItemChoiceType7 = RTGS.Models.ISO20022.CustomerCreditTransfer.ItemChoiceType7;
using ItemChoiceType8 = RTGS.Models.ISO20022.CustomerCreditTransfer.ItemChoiceType8;
using ItemChoiceType9 = RTGS.Models.ISO20022.CustomerCreditTransfer.ItemChoiceType9;
using LocalInstrument2Choice = RTGS.Models.ISO20022.CustomerCreditTransfer.LocalInstrument2Choice;
using OrganisationIdentification29 = RTGS.Models.ISO20022.CustomerCreditTransfer.OrganisationIdentification29;
using OrganisationIdentificationSchemeName1Choice = RTGS.Models.ISO20022.CustomerCreditTransfer.OrganisationIdentificationSchemeName1Choice;
using Party38Choice = RTGS.Models.ISO20022.CustomerCreditTransfer.Party38Choice;
using PartyIdentification135 = RTGS.Models.ISO20022.CustomerCreditTransfer.PartyIdentification135;
using RemittanceInformation16 = RTGS.Models.ISO20022.CustomerCreditTransfer.RemittanceInformation16;

namespace RTGS.Utils;

public static class ModelUtils
{
    public static Transaction ToTransaction(TransferRequest request)
    {
        return new Transaction
        {
            TransactionId = request.Id,
            PartnerId = request.PartnerId,
            MessageType = MessageType.Wallet
        };
    }

    private static AppHeader ToAppHeader(this TransferRequest request, string msgId)
    {
        return new AppHeader
        {
            Fr = new Party9Choice
            {
                Item = new BranchAndFinancialInstitutionIdentification5
                {
                    FinInstnId = new FinancialInstitutionIdentification8
                    {
                        ClrSysMmbId = new ClearingSystemMemberIdentification2 { MmbId = request.DbtrBic }
                    }
                }
            },
            To = new Party9Choice
            {
                Item = new BranchAndFinancialInstitutionIdentification5
                {
                    FinInstnId = new FinancialInstitutionIdentification8
                    {
                        ClrSysMmbId = new ClearingSystemMemberIdentification2 { MmbId = $"TJ{request.NbtBic}" }
                    }
                }
            },
            BizMsgIdr = msgId,
            MsgDefIdr = "pacs.008.001.08",
            BizSvc = "RTGS",
            CreDt = DateTime.Now,
        };
    }

    private static CustomerCreditTransfer ToCreditTransfer(this TransferRequest request, string msgId)
    {
        var groupHeader = new Models.ISO20022.CustomerCreditTransfer.GroupHeader93
        {
            MsgId = msgId,
            CreDtTm = DateTime.Now,
            BtchBookg = false,
            NbOfTxs = "1",
            TtlIntrBkSttlmAmt = new ActiveCurrencyAndAmount {Ccy = "TJS", Value = request.Amount},
            SttlmInf = new Models.ISO20022.CustomerCreditTransfer.SettlementInstruction7 { SttlmMtd = Models.ISO20022.CustomerCreditTransfer.SettlementMethod1Code.CLRG }
        };

        var cdtTrfTxInf = new CreditTransferTransaction39
        {
            PmtId = new Models.ISO20022.CustomerCreditTransfer.PaymentIdentification7
            {
                InstrId = msgId, 
                EndToEndId = $"{msgId}/VASL", 
                TxId = request.Id
            },
            PmtTpInf = new Models.ISO20022.CustomerCreditTransfer.PaymentTypeInformation28
            {
                ClrChanl = Models.ISO20022.CustomerCreditTransfer.ClearingChannel2Code.RTGS, 
                SvcLvl = [new Models.ISO20022.CustomerCreditTransfer.ServiceLevel8Choice
                {
                    Item = "0019", 
                    ItemElementName = ItemChoiceType6.Prtry
                }],
                LclInstrm = new LocalInstrument2Choice{
                    Item = "RTGS-SSCT",
                    ItemElementName = ItemChoiceType7.Prtry
                },
                CtgyPurp = new Models.ISO20022.CustomerCreditTransfer.CategoryPurpose1Choice
                {
                    Item = "001",
                    ItemElementName = ItemChoiceType8.Prtry
                }
            },
            IntrBkSttlmAmt = new ActiveCurrencyAndAmount {Ccy = "TJS", Value = request.Amount},
            IntrBkSttlmDt = DateTime.Now,
            ChrgBr = ChargeBearerType1Code.SHAR,
            InstgAgt = new BranchAndFinancialInstitutionIdentification6
            {
                FinInstnId = new FinancialInstitutionIdentification18
                {
                    ClrSysMmbId = new RTGS.Models.ISO20022.CustomerCreditTransfer.ClearingSystemMemberIdentification2 { MmbId = $"TJ{request.DbtrBic}" }
                }
            },
            InstdAgt = new BranchAndFinancialInstitutionIdentification6
            {
                FinInstnId = new FinancialInstitutionIdentification18
                {
                    ClrSysMmbId = new RTGS.Models.ISO20022.CustomerCreditTransfer.ClearingSystemMemberIdentification2 { MmbId = $"TJ{request.CdtrBic}" }
                }
            },
            Dbtr = new PartyIdentification135
            {
                Nm = request.DbtrOrgName,
                Id = new Party38Choice
                {
                    Item = new OrganisationIdentification29
                    {
                        Othr = [new GenericOrganisationIdentification1
                        {
                            Id = request.DbtrInn, 
                            SchmeNm = new OrganisationIdentificationSchemeName1Choice
                            {
                                Item = "INN", 
                                ItemElementName = ItemChoiceType9.Prtry
                            }
                        }]
                    }
                }
            },
            DbtrAcct = new CashAccount38
            {
                Id = new AccountIdentification4Choice
                {
                    Item = new GenericAccountIdentification1
                    {
                        Id = request.DbtrAcct,
                    }
                }
            },
            DbtrAgt = new BranchAndFinancialInstitutionIdentification6
            {
                FinInstnId = new FinancialInstitutionIdentification18
                {
                    ClrSysMmbId = new Models.ISO20022.CustomerCreditTransfer.ClearingSystemMemberIdentification2
                    {
                        MmbId = $"TJ{request.DbtrBic}"
                    }
                }
            },
            DbtrAgtAcct = new CashAccount38
            {
                Id = new AccountIdentification4Choice
                {
                    Item = new GenericAccountIdentification1
                    {
                        Id = request.DbtrAgtAcct,
                    }
                }
            },
            CdtrAgt = new BranchAndFinancialInstitutionIdentification6
            {
                
                FinInstnId = new FinancialInstitutionIdentification18
                {
                    ClrSysMmbId = new Models.ISO20022.CustomerCreditTransfer.ClearingSystemMemberIdentification2
                    {
                        MmbId = $"TJ{request.CdtrBic}"
                    }
                }
            },
            CdtrAgtAcct = new CashAccount38
            {
                Id = new AccountIdentification4Choice
                {
                    Item = new GenericAccountIdentification1
                    {
                        Id = request.CdtrAgtAcct,
                    }
                }
            },
            Cdtr = new PartyIdentification135
            {
                Nm = request.CdtrOrgName,
                Id = new Party38Choice
                {
                    Item = new OrganisationIdentification29
                    {
                        Othr = [new GenericOrganisationIdentification1
                        {
                            Id = request.CdtrInn, 
                            SchmeNm = new OrganisationIdentificationSchemeName1Choice
                            {
                                Item = "INN", 
                                ItemElementName = ItemChoiceType9.Prtry
                            }
                        }]
                    }
                }
            },
            CdtrAcct = new CashAccount38
            {
                Id = new AccountIdentification4Choice
                {
                    Item = new GenericAccountIdentification1
                    {
                        Id = request.CdtrAcct,
                    }
                }
            },
            RgltryRptg = [new RegulatoryReporting3
            {
                Dtls = [new StructuredRegulatoryReporting3
                {
                    Inf = [request.SerialNumber, DateTime.Now.ToShortDateString(), "0"]
                }]
            }],
            RmtInf = new RemittanceInformation16
            {
                Ustrd = [request.Comment]
            }
        };
        
        return new CustomerCreditTransfer
        {
            FIToFICstmrCdtTrf = new FIToFICustomerCreditTransferV08
            {
                CdtTrfTxInf = [cdtTrfTxInf],
                GrpHdr = groupHeader,
            }
        };
    }

    public static DataPdu ToDataPdu(this TransferRequest request)
    {
        var msgId = GenerateMessageId(request.Id);
        var appHeader = request.ToAppHeader(msgId);
        var transfer = request.ToCreditTransfer(msgId);
        
        return new DataPdu { Body = new Body { AppHdr = appHeader, CustomerCreditTransfer = transfer, } };
    }

    public static Transaction ToTransaction(this CustomerCreditTransfer customerCreditTransfer)
    {
        return new Transaction
        {
            TransactionId = customerCreditTransfer.FIToFICstmrCdtTrf.CdtTrfTxInf[0].PmtId.TxId,
            MessageType = MessageType.Card,
            PartnerId = 0
        };
    }
    
    public static Transaction ToTransaction(this InstitutionCreditTransfer institutionCreditTransfer)
    {
        return new Transaction
        {
            TransactionId = institutionCreditTransfer.FICdtTrf.CdtTrfTxInf[0].PmtId.TxId,
            MessageType = MessageType.Card,
            PartnerId = 0
        };
    }

    public static string FileName(this Transaction transaction)
    {
        return $"{transaction.TransactionId}.xml";
    }
    
    private static string GenerateMessageId(string requestId)
    {
        if (string.IsNullOrWhiteSpace(requestId))
            throw new ArgumentException("Request ID cannot be null or empty.", nameof(requestId));
        return $"008{DateTime.Now:yyyyMMdd}{requestId}";
    }

    // TODO add transactions from camt.053
    /*public static List<Transaction> ToTransactionsList(this Statement statement)
    {
        var transactions = new List<Transaction>();
        var numberOfEntries = int.Parse(statement.BkToCstmrStmt.Stmt[0].TxsSummry.TtlNtries.NbOfNtries);
        var entries = statement.BkToCstmrStmt.Stmt[0].Ntry;
        foreach (var entry in entries)
        {
            transactions.Add(entry.ToTransaction());
        }
    }*/
}