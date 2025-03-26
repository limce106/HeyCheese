# HeyCheese

<br/>

## 깃허브 커밋 규칙
- Ref: [Git Commit Message Convention](https://github.com/gyoogle/tech-interview-for-developer/blob/master/ETC/Git%20Commit%20Message%20Convention.md)

**커밋 메세지 형식**
> type: Subject (제목)
> <br/>
> body (본문)
> <br/>
> footer (꼬리말)

- `feat` : 새로운 기능에 대한 커밋
- `fix` : 버그 수정에 대한 커밋
- `build` : 빌드 관련 파일 수정에 대한 커밋
- `chore` : 그 외 자잘한 수정에 대한 커밋
- `ci` : CI관련 설정 수정에 대한 커밋
- `docs` : 문서 수정에 대한 커밋
- `style` : 코드 스타일 혹은 포맷 등에 관한 커밋
- `refactor` : 코드 리팩토링에 대한 커밋
- `test` : 테스트 코드 수정에 대한 커밋

**Subject (제목)**

- *한글*로 간결하게 작성

**Body (본문)**

- 상세히 작성, 기본적으로 무엇을 왜 진행 하였는지 작성
- Issue 등록 시, Issue 태그

**footer (꼬리말)**

- 참고사항

<br/>

## 깃허브 브랜치 규칙
- Ref: [Git Branch & Naming](https://ej-developer.tistory.com/75)

**크게 3가지 유형의 브랜치로 분기하여 사용**

- `main` : 유저에게 배포가능한 상태를 관리하는 브랜치. 절대 함부로 병합 시키지 말것
- `develop` : 기능개발을 위한 브랜들을 병합시키는 브랜치. feature/... 브랜치는 이곳에서 분기하여 병합, 안정적인 상태일때, main에 병합
- `feature/...` : 새로운 기능 및 버그 수정이 필요할 때 사용하는 브랜치. develop 브랜치에서 분기하여 병합, 더 이상 필요가 없다면 삭제 naming ex) feature/dialogue ex) main -> develop -> feature 분기 feature -> develop -> main 병합

## PR 규칙
**PR 제목**: 03.26 작업 제목

**Comment**

- 작업에 대한 자세한 내용(이유)
- 작업 내용
- 미리보기(첨부)
<img width="640" alt="img1 daumcdn" src="https://github.com/user-attachments/assets/feda2e1e-8965-46fc-8548-48fc970402c8" />
