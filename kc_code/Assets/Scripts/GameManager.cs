using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("--- UI �ؽ�Ʈ ���� (TMP) ---")]
    public TextMeshProUGUI monthText;      // ���: 1������
    public TextMeshProUGUI cashText;       // ���: ����
    public TextMeshProUGUI savingsText;    // ���: ����
    public TextMeshProUGUI loanText;       // ���: ��
    public TextMeshProUGUI stressText;     // ���: ��Ʈ����
    public TextMeshProUGUI salaryText;     // ���: ���� ����

    [Header("--- �˾� �г� ���� ---")]
    public GameObject bankPanel;           // ���� â
    public GameObject storePanel;          // ���� â
    public GameObject academyPanel;        // �п� â
    public GameObject reportPanel;         // ���� ������ â
    public GameObject salaryReportPanel;
    public TextMeshProUGUI salaryReportText;
    public Button closeReportBtn;
    public TextMeshProUGUI reportContent;  // ������ ����
    public GameObject resultPanel;         // ���� â
    public TextMeshProUGUI resultTitle;    // ���� ����
    public TextMeshProUGUI resultDesc;     // ���� ����
    public GameObject msgPanel;            // �˸� �佺Ʈ â
    public TextMeshProUGUI msgText;        // �˸� ����

    [Header("--- ��ư ���� ���� ---")]
    public Button savingsJoinBtn;          // ���� ���� ��ư (���� �� ��Ȱ��ȭ��)
    public TextMeshProUGUI savingsBtnText;

    [Header("--- ���� ������ ---")]
    public int currentMonth = 1;
    public long cash = 300000;
    public long savings = 0;
    public long loan = 0;
    public int stress = 0;
    public int jobLevel = 1;

    // �뷱�� ���
    private const int BASE_SALARY = 2000000;
    private const int MONTHLY_SAVINGS_AMOUNT = 500000;
    private const int LIVING_COST = 500000;
    private bool isSavingsJoined = false;

    void Awake() { instance = this; }
    void Start() { UpdateUI(); CloseAllPanels(); }

    // [���� ��] ��ư ���� �Լ�
    public void OnClickNextMonth()
    {
        if (currentMonth >= 10) { EndGame("����"); return; }

        int currentSalary = BASE_SALARY + ((jobLevel - 1) * 100000);

        // 1. ���� ��� (���ο���, �ǰ�����, �ҵ漼)
        int pension = (int)(currentSalary * 0.045f);
        int health = (int)(currentSalary * 0.035f);
        int tax = (int)(currentSalary * 0.03f);
        int totalDeduction = pension + health + tax;
        int netPay = currentSalary - totalDeduction;

        // 2. ���� ���� (2%)
        int loanInterest = 0;
        if (loan > 0)
        {
            loanInterest = (int)(loan * 0.02f);
            netPay -= loanInterest;
        }

        // 3. ���� �ڵ���ü
        string savingsMsg = "";
        if (isSavingsJoined)
        {
            if (cash + netPay >= MONTHLY_SAVINGS_AMOUNT)
            {
                savings += MONTHLY_SAVINGS_AMOUNT;
                netPay -= MONTHLY_SAVINGS_AMOUNT;
                savingsMsg = $"<color=blue>���� �ڵ���ü: -{MONTHLY_SAVINGS_AMOUNT:N0}</color>\n";
            }
            else savingsMsg = $"<color=red>* �ܾ� ���� ��ü ����</color>\n";
        }

        // 4. ���� ���� ���� (0.5%)
        int interest = (int)(savings * 0.005f);
        savings += interest;

        // 5. ���� �ݿ�
        cash += netPay;
        cash -= LIVING_COST;

        // 6. ��Ʈ���� ����
        stress += 20;
        if (stress > 100) stress = 100;

        // ������ �ؽ�Ʈ �ۼ�
        string report = $"<size=120%><b>{currentMonth}�� �޿� ������</b></size>\n\n" +
                        $"���� ����: {currentSalary:N0}\n" +
                        $"--------------------\n" +
                        $"<color=red>���ο���(4.5%): -{pension:N0}</color>\n" +
                        $"<color=red>�ǰ�����(3.5%): -{health:N0}</color>\n" +
                        $"<color=red>�ҵ漼 ��: -{tax:N0}</color>\n" +
                        (loan > 0 ? $"<color=red>���� ����(2%): -{loanInterest:N0}</color>\n" : "") +
                        $"--------------------\n" +
                        $"<b>�Ǽ��ɾ�: {(currentSalary - totalDeduction - loanInterest):N0}</b>\n\n" +
                        $"<color=red>���� ��Ȱ��: -{LIVING_COST:N0}</color>\n" +
                        savingsMsg +
                        $"<color=blue>���� ���� ����: +{interest:N0}</color>\n" +
                        $"--------------------\n" +
                        $"<b>���� ���� ����: {(netPay - LIVING_COST):N0}</b>";

        reportContent.text = report;
        reportPanel.SetActive(true);

        currentMonth++;

        if (stress >= 100) EndGame("��Ʈ����");
        else if (cash < 0) EndGame("�Ļ�");

        UpdateUI();
    }

    // --- �ൿ �Լ� (��ư �����) ---
    public void ActionBorrow() // ���� �ޱ�
    {
        if (loan >= 2000000) { ShowToast("�ѵ� �ʰ�!"); return; }
        loan += 500000; cash += 500000;
        UpdateUI(); ShowToast("50���� ���� �Ϸ�"); bankPanel.SetActive(false);
    }

    public void ActionRepay() // ���� ��ȯ
    {
        if (loan <= 0) { ShowToast("���� ���� �����ϴ�."); return; }
        if (cash < 500000) { ShowToast("������ �����մϴ�."); return; }
        loan -= 500000; cash -= 500000;
        UpdateUI(); ShowToast("50���� ��ȯ �Ϸ�"); bankPanel.SetActive(false);
    }

    public void ActionJoinSavings() // ���� ����
    {
        if (isSavingsJoined) return;
        isSavingsJoined = true;
        UpdateUI(); ShowToast("���� ���� �Ϸ�!"); bankPanel.SetActive(false);
    }

    public void ActionBuy() // �Һ� (����)
    {
        if (cash < 50000) { ShowToast("���� �����մϴ�."); return; }
        cash -= 50000; stress -= 30; if (stress < 0) stress = 0;
        UpdateUI(); ShowToast("���� �Ϸ�!"); storePanel.SetActive(false);
    }

    public void ActionStudy() // ���� (�п�)
    {
        if (cash < 100000) { ShowToast("������ ����!"); return; }
        cash -= 100000; jobLevel++; stress += 10; if (stress > 100) stress = 100;
        UpdateUI(); ShowToast("���� ���� ���!"); academyPanel.SetActive(false);
    }

    public void ActionRest() // �޽� (�п�/��)
    {
        stress -= 10; if (stress < 0) stress = 0;
        UpdateUI(); ShowToast("�޽� �Ϸ�"); academyPanel.SetActive(false);
    }

    public void CloseAllPanels()
    {
        if (bankPanel) bankPanel.SetActive(false);
        if (storePanel) storePanel.SetActive(false);
        if (academyPanel) academyPanel.SetActive(false);
        if (reportPanel) reportPanel.SetActive(false);
        if (resultPanel) resultPanel.SetActive(false);
        if (msgPanel) msgPanel.SetActive(false);
    }

    void UpdateUI()
    {
        monthText.text = $"{currentMonth}������";
        cashText.text = $"{cash:N0}";
        savingsText.text = $"{savings:N0}";
        loanText.text = $"{loan:N0}";
        stressText.text = $"{stress}%";
        stressText.color = stress > 80 ? Color.red : Color.white;
        int curSal = BASE_SALARY + ((jobLevel - 1) * 100000);
        salaryText.text = $"{curSal:N0}";

        if (isSavingsJoined && savingsJoinBtn != null)
        {
            savingsJoinBtn.interactable = false;
            if (savingsBtnText != null) savingsBtnText.text = "���� �Ϸ�";
        }
    }

    void ShowToast(string msg)
    {
        StopAllCoroutines();
        StartCoroutine(ToastProcess(msg));
    }

    IEnumerator ToastProcess(string msg)
    {
        msgPanel.SetActive(true);
        msgText.text = msg;
        yield return new WaitForSeconds(2f);
        msgPanel.SetActive(false);
    }

    void EndGame(string type)
    {
        resultPanel.SetActive(true);
        long total = cash + savings - loan;
        string title = "", desc = "";

        if (type == "��Ʈ����") { title = "���� ����"; desc = "��Ʈ���� ���ٷ� ���������ϴ�."; }
        else if (type == "�Ļ�") { title = "�Ļ�"; desc = "���� ���� ���߽��ϴ�."; }
        else
        {
            title = "���� ����";
            desc = $"���� ����: {cash:N0}\n���� ����: {savings:N0}\n���� ����: -{loan:N0}\n\n<b>���ڻ�: {total:N0}</b>";
        }
        resultTitle.text = title;
        resultDesc.text = desc;
    }
}