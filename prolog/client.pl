:- dynamic action/2, lastAction/1.
%--------------------------------------------------------------------%
start :-
    agent_reset,
    %agent_init,
    connect,
    run,
    disconnect.

%--------------------------------------------------------------------%
connect :-
    IP = '127.0.0.1',
    Port = 8888,
    connect(IP, Port).

connect(IP, Port) :-
    tcp_socket(Socket),
    tcp_connect(Socket, IP:Port),
    tcp_open_socket(Socket, InStream, OutStream),
    retractall(connection(_, _, _)),
    assert(connection(Socket, InStream, OutStream)),
    !,
    writef('CONNECTION: Connected to %w on port %w.\n', [IP, Port]),
    name(Name),
    authenticate(Name, success),
    write('CONNECTION: Successfully authenticated. Ready to receive percepts.'), nl,
    !.
connect(IP, Port) :-
    writef('CONNECTION ERROR: Connection to %w:%w failure!\n', [IP, Port]),
    fail.

%--------------------------------------------------------------------%
authenticate(Name, Result) :-
    connection(_, InStream, OutStream),

    % Send agent name.
    swritef(Authentication, '<authentication><name>%w</name></authentication>', [Name]),
    write_term(OutStream, Authentication, []),
    nl(OutStream),
    flush_output(OutStream),

    % Receive result.
    read(InStream, Result),
    write('CONNECTION: authentication result: '), write(Result), nl.

%--------------------------------------------------------------------%
disconnect :-
    % Flush the percept message from the stream.
    recv_percept(_), 

    connection(Socket, InStream, OutStream),

    % Send goodbye message.
    send_action(action(goodbye, []), success),

    tcp_close_socket(Socket),
    retractall(connection(Socket, InStream, OutStream)),
    write('CONNECTION: Disconnected.\n'),
    !.
disconnect :- 
    write('CONNECTION: Disconnect failure!\n'),
    fail.

%--------------------------------------------------------------------%
recv_percept(Percept) :-
    connection(_, InStream, _),
    read(InStream, Percept),
    Percept \= end_of_file,
    !.
recv_percept(_) :-
    write('CONNECTION: failure to recieve percept!'), nl,
    fail.

%--------------------------------------------------------------------%
%% Valid actions:
%%     action(ID, goodbye, [])
%%     action(ID, noop,    [])
%%     action(ID, move,    [Position])
%%     action(ID, attack,  [Agent])
%%     action(ID, pickup,  [Object])
%%     action(ID, drop,    [Object])
%%
%% where
%%     Position = [Integer, Integer]
%%     Object   = Integer
%%     Agent    = Atom
%%     Result   = success | failure
send_action(Action, Result) :-
    action_id(ID),
    action_to_xml(Action, ID, XML),

    % Write action to socket. 
    connection(_, InStream, OutStream),
    write_term(OutStream, XML, []),
    nl(OutStream),
    flush_output(OutStream),
    % Receive action result.
    read(InStream, Response),	
    action_result(Response, Result).

action_result(unknown, unknown).
action_result(success, success):-
	lastAction(LastAction),
	assert(LastAction).
action_result(failure, failure) :-
    write('CONNECTION: action failed!'), nl.
action_result(end_of_file, failure) :-
    write('CONNECTION ERROR: connection closed!'), nl,
    fail.

%--------------------------------------------------------------------%
action_to_xml(action(goodbye, []), ID, XML) :-
    swritef(XML, '<action><id>%w</id><type>goodbye</type></action>', [ID]),
    !.
action_to_xml(action(noop, []), ID, XML) :-
    swritef(XML, '<action><id>%w</id><type>noop</type></action>', [ID]),
    !.
action_to_xml(action(move, [Position]), ID, XML) :-
    swritef(XML, '<action><id>%w</id><type>move</type><position>%w</position></action>', [ID, Position]),
    !.
action_to_xml(action(attack, [Agent]), ID, XML) :-
    swritef(XML, '<action><id>%w</id><type>attack</type><agent><id>%w</id></agent></action>', [ID, Agent]),
    !.
action_to_xml(action(pickup, [Object]), ID, XML) :-
    swritef(XML, '<action><id>%w</id><type>pickup</type><object><id>%w</id></object></action>', [ID, Object]),
    !.
action_to_xml(action(drop, [Object]), ID, XML) :-
    swritef(XML, '<action><id>%w</id><type>drop</type><object><id>%w</id></object></action>', [ID, Object]),
    !.
action_to_xml(_, _, _) :-
    write('CONNECTION ERROR: Invalid action.'), nl,
    fail.

%--------------------------------------------------------------------%
run :-
    write('%--------------------------------------------------------------------%'), nl,
    write('AGENT: receiving percept...'), nl, 
    recv_percept(Percept),
    agent(Percept, Action),
    write('AGENT: sending action... '), write(Action), nl,
    send_action(Action, Result),
    write('AGENT: result: '), write(Result), nl, nl,
    run.

%--------------------------------------------------------------------%
action_id(ID) :-
    current_action_id(ID),
    NextID is ID + 1,
    retractall(current_action_id(_)),
    assert(current_action_id(NextID)).

%--------------------------------------------------------------------%
agent_reset :-
    retractall( current_action_id(_) ),
    assert(     current_action_id(0) ).

%--------------------------------------------------------------------%
%% This predicate must be implemented by the students
agent_init(Name) :-
    retractall( name(_)              ),
    assert(     name(Name)           ).

%--------------------------------------------------------------------%
agent(Percept, Action) :-
    write('AGENT: percept: '), write(Percept), nl,
    write('AGENT: thinking...'), nl,
	plan(Percept, Action),
	retractall(lastAction(_)),
	assert(lastAction(Action)),
	writeln(Action).
	
%if the agent has an object in the backpack, he drops it
plan(Percept, action(drop, [Name])):-
	name(Self),
	member(selfProperties(Self, _, _, _, [entity(Name, gold, _, _, _) | _], _), Percept),
	!.
	
%% The agent is in the same node as the gold
plan(Percept, action(pickup, [Name])):-
	name(Self),
	member(entity(Name, gold, Node, _P, _Prop), Percept),
	member(entity(Self, agent, Node, _, _), Percept),
	not(action(pickup, [Name])),
	!.

%% The agent sees a gold, and moves near it
plan(Percept, action(move, [Node])):-
	name(Self),
	member(entity(Self, agent, ActualNode, _, _), Percept),
	member(entity(_Name, gold, Node, _P, _Prop), Percept),
	member(node(Node, _Pos, Connections), Percept), 
	member(ActualNode, Connections),
	!.

%% The agent moves randomly
plan(Percept, action(move, [Node])):-
	name(Self),
	member(entity(Self, agent, ActualNode, _, _), Percept),
	findall(
		node(Name, Pos, Connections), 
		(
			member(node(Name, Pos, Connections), Percept), 
			member(ActualNode, Connections)
		),
		Nodes),
	random_member(node(Node, _, _), Nodes).
