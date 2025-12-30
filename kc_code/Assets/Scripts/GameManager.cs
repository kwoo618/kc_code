using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("--- 상단 HUD UI (TMP) ---")]
    public TextMeshProUGUI monthText;      // 1개월차
    public TextMeshProUGUI cashText;       // 소지금
    public TextMeshProUGUI savingsText;    // 적금액
    public TextMeshProUGUI loanText;       // 대출금
    public TextMeshProUGUI stressText;     // 스트레스
    public TextMeshProUGUI salaryText;     // 현재 월급 표시

    [Header("--- 팝업 패널 리스트 ---")]
    public GameObject bankPanel;           // 은행 창
    public GameObject storePanel;          // 상점 창
    public GameObject academyPanel;        // 학원 창
    public GameObject reportPanel;         // [중요] 급여 명세서(보고서) 창
    public TextMeshProUGUI reportContent;  // 명세서 내용 텍스트
    public GameObject resultPanel;         // 게임 종료 결과 창
    public TextMeshProUGUI resultTitle;    
    public TextMeshProUGUI resultDesc;     
    public GameObject msgPanel;            // 토스트 알림 창
    public TextMeshProUGUI msgText;        

    [Header("--- 버튼 관련 ---")]
    public Button savingsJoinBtn;          
    public TextMeshProUGUI savingsBtnText;

    [Header("--- 게임 데이터 ---")]
    public int currentMonth = 1;
    public long cash = 300000;
    public long savings = 0;
    public long loan = 0;
    public int stress = 0;
    public int jobLevel = 1;

    private const int BASE_SALARY = 2000000;
    private const int MONTHLY_SAVINGS_AMOUNT = 500000;
    private const int LIVING_COST = 500000;
    private bool isSavingsJoined = false;

    void Awake() { instance = this; }
    void Start() { UpdateUI(); CloseAllPanels(); }

    public void OnCarAccident()
    {
        // 교통사고 패널티: 50,000원 차감 (원하는 금액으로 수정 가능)
        int penalty = 50000;

        // 현금이 부족하면 0까지만 차감
        if (cash >= penalty) cash -= penalty;
        else cash = 0;

        // 스트레스 증가
        stress += 10;
        if (stress > 100) stress = 100;

        ShowToast($"교통사고! 치료비 -{penalty:N0}원");
        UpdateUI();
    }

    // [다음 달로] 버튼 클릭 시 호출
    public void OnClickNextMonth()
    {
        if (currentMonth >= 12) { EndGame("완료"); return; }

        int currentSalary = BASE_SALARY + ((jobLevel - 1) * 100000);

        // 1. 공제액 계산
        int pension = (int)(currentSalary * 0.045f);
        int health = (int)(currentSalary * 0.035f);
        int tax = (int)(currentSalary * 0.03f);
        int totalDeduction = pension + health + tax;
        int netPay = currentSalary - totalDeduction;

        // 2. 대출 이자 (2%)
        int loanInterest = 0;
        if (loan > 0)
        {
            loanInterest = (int)(loan * 0.02f);
            netPay -= loanInterest;
        }

        // 3. 적금 자동이체
        string savingsMsg = "";
        if (isSavingsJoined)
        {
            if (cash + netPay >= MONTHLY_SAVINGS_AMOUNT)
            {
                savings += MONTHLY_SAVINGS_AMOUNT;
                netPay -= MONTHLY_SAVINGS_AMOUNT;
                savingsMsg = $"<color=blue>▶ 적금 이체: -{MONTHLY_SAVINGS_AMOUNT:N0}</color>\n";
            }
            else savingsMsg = $"<color=red>▶ 잔액부족 이체실패</color>\n";
        }

        // 4. 적금 이자 수익 (0.5%)
        int interest = (int)(savings * 0.005f);
        savings += interest;

        // 5. 최종 소지금 반영 (월급 추가 - 생활비 차감)
        cash += netPay;
        cash -= LIVING_COST;

        // 6. 스트레스 증가
        stress += 20;
        if (stress > 100) stress = 100;

        // 명세서 텍스트 구성 (Report_Panel 전용)
        string report = $"<size=130%><b>[{currentMonth}월 급여 명세서]</b></size>\n\n" +
                        $"기본 급여: +{currentSalary:N0}\n" +
                        $"--------------------------------\n" +
                        $"<color=red>국민연금: -{pension:N0}</color>\n" +
                        $"<color=red>건강보험: -{health:N0}</color>\n" +
                        $"<color=red>소득세: -{tax:N0}</color>\n" +
                        (loan > 0 ? $"<color=red>대출이자: -{loanInterest:N0}</color>\n" : "") +
                        $"--------------------------------\n" +
                        $"<b>실수령액: {(currentSalary - totalDeduction - loanInterest):N0}</b>\n\n" +
                        $"고정 생활비: -{LIVING_COST:N0}\n" +
                        savingsMsg +
                        $"적금 이자수익: +{interest:N0}\n" +
                        $"--------------------------------\n" +
                        $"<b>최종 잔액변화: {(netPay - LIVING_COST):N0}</b>";

        reportContent.text = report;
        reportPanel.SetActive(true); // 스크린샷의 Report_Panel 활성화

        currentMonth++;

        if (stress >= 100) EndGame("스트레스");
        else if (cash < 0) EndGame("파산");

        UpdateUI();
    }

    // --- 액션 함수들 ---
    public void ActionBorrow() {
        if (loan >= 2000000) { ShowToast("대출 한도 초과!"); return; }
        loan += 500000; cash += 500000;
        UpdateUI(); ShowToast("50만원 대출 완료"); bankPanel.SetActive(false);
    }

    public void ActionRepay() {
        if (loan <= 0) { ShowToast("상환할 대출이 없습니다."); return; }
        if (cash < 500000) { ShowToast("잔액이 부족합니다."); return; }
        loan -= 500000; cash -= 500000;
        UpdateUI(); ShowToast("50만원 상환 완료"); bankPanel.SetActive(false);
    }

    public void ActionJoinSavings() {
        if (isSavingsJoined) return;
        isSavingsJoined = true;
        UpdateUI(); ShowToast("적금 가입 완료!"); bankPanel.SetActive(false);
    }

    public void ActionBuy() {
        if (cash < 50000) { ShowToast("돈이 부족합니다."); return; }
        cash -= 50000; stress -= 30; if (stress < 0) stress = 0;
        UpdateUI(); ShowToast("아이템 구매 완료!"); storePanel.SetActive(false);
    }

    public void ActionStudy() {
        if (cash < 100000) { ShowToast("학원비가 부족합니다."); return; }
        cash -= 100000; jobLevel++; stress += 10; if (stress > 100) stress = 100;
        UpdateUI(); ShowToast("직무 능력 향상!"); academyPanel.SetActive(false);
    }

    public void ActionRest() {
        stress -= 10; if (stress < 0) stress = 0;
        UpdateUI(); ShowToast("휴식 완료"); academyPanel.SetActive(false);
    }

    public void CloseAllPanels() {
        if (bankPanel) bankPanel.SetActive(false);
        if (storePanel) storePanel.SetActive(false);
        if (academyPanel) academyPanel.SetActive(false);
        if (reportPanel) reportPanel.SetActive(false);
        if (resultPanel) resultPanel.SetActive(false);
        if (msgPanel) msgPanel.SetActive(false);
    }

    void UpdateUI() {
        monthText.text = $"{currentMonth}개월차";
        cashText.text = $"{cash:N0}";
        savingsText.text = $"{savings:N0}";
        loanText.text = $"{loan:N0}";
        stressText.text = $"{stress}%";
        stressText.color = stress > 80 ? Color.red : Color.white;
        int curSal = BASE_SALARY + ((jobLevel - 1) * 100000);
        salaryText.text = $"{curSal:N0}";

        if (isSavingsJoined && savingsJoinBtn != null) {
            savingsJoinBtn.interactable = false;
            if (savingsBtnText != null) savingsBtnText.text = "가입 완료";
        }
    }

    void ShowToast(string msg) {
        StopAllCoroutines();
        StartCoroutine(ToastProcess(msg));
    }

    IEnumerator ToastProcess(string msg) {
        msgPanel.SetActive(true);
        msgText.text = msg;
        yield return new WaitForSeconds(2f);
        msgPanel.SetActive(false);
    }

    void EndGame(string type) {
        resultPanel.SetActive(true);
        long total = cash + savings - loan;
        string title = "", desc = "";

        if (type == "스트레스") { title = "과로사"; desc = "스트레스 누적으로 병원에 실려갔습니다."; }
        else if (type == "파산") { title = "파산"; desc = "더 이상 낼 돈이 없습니다."; }
        else {
            title = "성공적인 퇴사";
            desc = $"최종 자산 현황\n현금: {cash:N0}\n적금: {savings:N0}\n대출: -{loan:N0}\n\n<b>총 자산: {total:N0}</b>";
        }
        resultTitle.text = title;
        resultDesc.text = desc;
    }
}