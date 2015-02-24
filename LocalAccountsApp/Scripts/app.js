function ViewModel() {
    var self = this;

    var tokenKey = 'accessToken';

    self.result = ko.observable();
    self.user = ko.observable();

    self.registerEmail = ko.observable();
    self.registerPassword = ko.observable();
    self.registerPassword2 = ko.observable();

    self.loginEmail = ko.observable("a@b.com");
	 
    self.loginPassword = ko.observable();

   

    function showError(jqXHR) {
        self.result(jqXHR.status + ': ' + jqXHR.statusText);
    }

    self.callApi = function () {
        self.result('');

        var token = sessionStorage.getItem(tokenKey);
        //var token = "JHduvrwAFImuBG5D-yiU7oskQ2z82djff66ZpMc2CbjCW2cQSftXcBTDOIwEC02Y1CsvCyI1rhaCWzEW0DmnXSBXLvNZPnzTpyf43n9Jk7pqgTYvdCkBQjwihY6kzfGnqruaT93yUmS3fRAR-b0jhz-eDkaGe90t4M0ESow4UOfbNnxniAUSi73YX4GOkpc7I7HFRzstJ1mocKi4UdxpRozroS-n1oFxy3QXkcBhpjLpwGUG4u_SPSX0LJZBJZdziBdG-M6bU-UHYpzL3MgTNr_B2i7tLjZJMVWkvwuZQwi3KZ0jgHzWXwFmkQfFK2E7TV-ANSm5Y1fEHfKigGSGbcEl799WqW68hp-LVynzIovFVTK_qLWl8jkz2rX-Nft-8URgwPXkP6DS52hkXFNnsnXodEhCVhv0tcf-qBn4mmLYNn1iTd_GGsk_MjHZZJOrieBR1xVQHyDeTEJIvCTCFOyzEkDNKltA4h6n-8dXfeoSZOoRM5IE7sng7o_NgTP689Mg3YOtVIaGxgxM8ANttw";
        var headers = {};
        if (token) {
            headers.Authorization = 'Bearer ' + token;
        }

        $.ajax({
            type: 'GET',
            url: '/api/values',
            headers: headers
        }).done(function (data) {
            self.result(data);
        }).fail(showError);
    }

    self.register = function () {
        self.result('');

        var data = {
            Email: self.registerEmail(),
            Password: self.registerPassword(),
            ConfirmPassword: self.registerPassword2()
        };

        $.ajax({
            type: 'POST',
            url: '/api/Account/Register',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(data)
        }).done(function (data) {
            self.result("Done!");
        }).fail(showError);
    }

    self.login = function () {
        self.result('');

        var loginData = {
            grant_type: 'password',
            username: self.loginEmail(),
            password: self.loginPassword()
        };

        $.ajax({
            type: 'POST',
            url: '/Token',
            data: loginData
        }).done(function (data) {
            self.user(data.userName);
          // Cache the access token in session storage.
           // alert("Token Issued Expires: " + data[".expires"]);
            sessionStorage.setItem(tokenKey, data.access_token);
        }).fail(showError);
    }

    self.logout = function () {
        self.user('');
        sessionStorage.removeItem(tokenKey)
    }
}
var app = new ViewModel();

ko.applyBindings(app);

