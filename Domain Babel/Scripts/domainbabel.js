(function () {
    "use strict";

    var ViewModel = function () {
        var self = this;

        self.ellipsis = ko.observable(null);

        self.ellipsis.subscribe(function (e) {
            if (self.searching()) {
                setTimeout(function () {
                    self.ellipsis(e.length === 3 ? '' : e + '.');
                }, 500);
            }
        });

        self.searching = ko.observable(false);

        self.searching.subscribe(function (s) {
            $('#txtSearch').attr('disabled', s);
            $('#btnSearch').button(s ? 'loading' : 'reset');

            if (s) {
                self.searchStartTime = Date.now();
                self.ellipsis('');
            }
            else {
                var duration = Date.now() - self.searchStartTime;
                ga('send', 'event', 'search', 'complete', { 'dimension1': $('#txtSearch').val(), 'metric1': self.searchResults().length, 'metric2': duration });
            }
        });

        self.searchStartTime = 0;

        self.languageCount = ko.observable(null);

        self.searchResults = ko.observableArray();

        self.sortedSearchResults = ko.computed(function () {
            return self.searchResults().sort(self.sortResults);
        });

        self.sortResults = function (l, r) {
            return (l.Part === r.Part ?
                    (l.Language > r.Language ? 1 : -1) :
                    (l.Part > r.Part ? 1 : -1));
        };

        self.hideResults = ko.computed(function () {
            return !self.searching() && self.searchResults().length === 0;
        });

        self.showSearchResult = function (el) { if (el.nodeType === 1) $(el).hide().slideDown() };

    }, viewModel;

    viewModel = new ViewModel();

    $(document).ready(function () {
        ko.bindingHandlers.fadeVisible = {
            update: function (element, valueAccessor) {
                var value = valueAccessor();
                if (ko.utils.unwrapObservable(value)) {
                    if ($(element).is(':hidden')) {
                        $(element).hide().removeClass('hide').slideDown(500);
                    }
                }
                else {
                    $(element).hide();
                }
            }
        };

        ko.applyBindings(viewModel, document.getElementById('searchResults'));

        var domain = $.connection.domainHub;

        domain.client.languageCount = function (count) {
            viewModel.languageCount(count);
        };

        domain.client.addResult = function (part, language, translation) {
            viewModel.searchResults.push({ Part: part.toLowerCase(), Language: language, Translation: translation.toLowerCase() });
        };

        domain.client.stop = function () {
            $.connection.hub.stop();

            viewModel.searching(false);
        };

        $('#txtSearch').keydown(function (e) {
            if (e.keyCode === 13) {
                $('#btnSearch').click();
                return false;
            }
        });

        $('#btnSearch').click(function (e) {
            viewModel.languageCount(null);
            viewModel.searchResults.removeAll();

            $(this).closest('form').validate();

            if ($(this).closest('form').valid()) {
                viewModel.searching(true);

                $.connection.hub.start().done(function () {
                    domain.server.search($('#txtSearch').val());
                });

                ga('send', 'event', 'search', 'start', { 'dimension1': $('#txtSearch').val() });
            }

            e.preventDefault();
            return false;
        });
    });
}());