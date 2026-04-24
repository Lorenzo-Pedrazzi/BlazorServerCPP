# Org.OpenAPITools.Api.LicenceApi

All URIs are relative to *https://localhost:5001*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**ActivateLicence**](LicenceApi.md#activatelicence) | **POST** /api/licence/activate | Attiva la licenza del cliente alla prima comunicazione col server |
| [**CheckLicenceState**](LicenceApi.md#checklicencestate) | **GET** /api/licence/state | Verifica lo stato corrente di una licenza |
| [**GetNewLicence**](LicenceApi.md#getnewlicence) | **POST** /api/licence/new | Rilascia una nuova licenza al cliente come file da scaricare |

<a id="activatelicence"></a>
# **ActivateLicence**
> Licenza ActivateLicence (ActivateLicenceRequest activateLicenceRequest)

Attiva la licenza del cliente alla prima comunicazione col server

Invocata la prima volta che il client comunica col server. Genera una nuova licenza associata al cliente, con identificativo alfanumerico composto da numero commessa (LDK) + data/ora di attivazione + id cliente, e scadenza calcolata dal server.


### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **activateLicenceRequest** | [**ActivateLicenceRequest**](ActivateLicenceRequest.md) |  |  |

### Return type

[**Licenza**](Licenza.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Licenza attivata con successo |  -  |
| **400** | Richiesta non valida |  -  |
| **404** | Cliente non trovato |  -  |
| **409** | Una licenza attiva esiste gia&#39; per il cliente |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="checklicencestate"></a>
# **CheckLicenceState**
> LicenceStateResponse CheckLicenceState (string licenceId)

Verifica lo stato corrente di una licenza

Restituisce lo stato della licenza identificata da licenceId: validita', data di scadenza, flag di pagamento. Puo' essere usata anche come heartbeat del client (monitoraggio ogni 5 minuti).


### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **licenceId** | **string** | Identificativo alfanumerico della licenza |  |

### Return type

[**LicenceStateResponse**](LicenceStateResponse.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Stato della licenza |  -  |
| **404** | Licenza non trovata |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="getnewlicence"></a>
# **GetNewLicence**
> System.IO.Stream GetNewLicence (GetNewLicenceRequest getNewLicenceRequest)

Rilascia una nuova licenza al cliente come file da scaricare

Invocata dal client per ottenere una nuova licenza (p.es. dopo scadenza e pagamento avvenuto). Il server genera il file di licenza in locale e lo restituisce al client come download binario. I metadati della licenza (id, scadenza, pagato) vengono propagati tramite response header, mentre il body contiene il file stesso.


### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **getNewLicenceRequest** | [**GetNewLicenceRequest**](GetNewLicenceRequest.md) |  |  |

### Return type

**System.IO.Stream**

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/octet-stream, application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | File di licenza generato |  * Content-Disposition - Nome del file di licenza proposto per il salvataggio <br>  * X-Licence-Id - Identificativo alfanumerico della licenza contenuta nel file <br>  * X-Licence-Scadenza - Data/ora di scadenza della licenza (ISO 8601) <br>  * X-Licence-Pagato - Indica se la licenza e&#39; stata pagata <br>  |
| **400** | Richiesta non valida |  -  |
| **402** | Licenza precedente non pagata, nuova emissione non consentita |  -  |
| **404** | Cliente non trovato |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

