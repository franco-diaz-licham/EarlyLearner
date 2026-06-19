begin;

insert into households (id, name, created_on)
values
    ('11111111-1111-1111-1111-111111111111', 'Demo Household', now())
on conflict (id) do nothing;

insert into carers (id, household_id, user_id, first_name, last_name, role, created_on)
values
    ('22222222-2222-2222-2222-222222222221', '11111111-1111-1111-1111-111111111111', '33333333-3333-3333-3333-333333333331', 'Avery', 'Taylor', 'Owner', now()),
    ('22222222-2222-2222-2222-222222222222', '11111111-1111-1111-1111-111111111111', '33333333-3333-3333-3333-333333333332', 'Jordan', 'Taylor', 'Caregiver', now())
on conflict (id) do nothing;

insert into children (id, household_id, given_name, date_of_birth, is_archived, created_on)
values
    ('44444444-4444-4444-4444-444444444441', '11111111-1111-1111-1111-111111111111', 'Mia', '2021-03-14', false, now()),
    ('44444444-4444-4444-4444-444444444442', '11111111-1111-1111-1111-111111111111', 'Noah', '2022-08-02', false, now())
on conflict (id) do nothing;

insert into readiness_outcomes (id, code, name, description, category, sort_order, status, created_on)
values
    ('55555555-5555-5555-5555-555555555551', 'language-listening', 'Listens and responds', 'Listens to short instructions and responds with words or actions.', 'Language', 10, 'Active', now()),
    ('55555555-5555-5555-5555-555555555552', 'social-turn-taking', 'Takes turns with others', 'Practises waiting, sharing, and taking turns during play.', 'Social', 20, 'Active', now()),
    ('55555555-5555-5555-5555-555555555553', 'motor-pencil-grip', 'Uses early mark making', 'Uses crayons, pencils, or brushes to make controlled marks.', 'Motor', 30, 'Active', now())
on conflict (id) do nothing;

insert into goals (id, household_id, child_id, title, type, status, timeframe_start_date, timeframe_end_date, created_on)
values
    ('66666666-6666-6666-6666-666666666661', '11111111-1111-1111-1111-111111111111', '44444444-4444-4444-4444-444444444441', 'Practise turn taking during games', 'ShortTerm', 'Active', '2026-06-15', '2026-06-21', now()),
    ('66666666-6666-6666-6666-666666666662', '11111111-1111-1111-1111-111111111111', '44444444-4444-4444-4444-444444444441', 'Build confidence with mark making', 'ShortTerm', 'Active', '2026-06-15', '2026-06-21', now())
on conflict (id) do nothing;

insert into goal_readiness_outcomes (goal_id, readiness_outcome_id)
values
    ('66666666-6666-6666-6666-666666666661', '55555555-5555-5555-5555-555555555552'),
    ('66666666-6666-6666-6666-666666666662', '55555555-5555-5555-5555-555555555553')
on conflict (goal_id, readiness_outcome_id) do nothing;

insert into learning_plans (id, household_id, child_id, period_start_date, period_end_date, focus, created_on)
values
    ('77777777-7777-7777-7777-777777777771', '11111111-1111-1111-1111-111111111111', '44444444-4444-4444-4444-444444444441', '2026-06-15', '2026-06-21', 'Simple play-based readiness practice', now())
on conflict (id) do nothing;

insert into planned_learning_sessions (id, learning_plan_id, planned_date, title, status, created_on)
values
    ('88888888-8888-8888-8888-888888888881', '77777777-7777-7777-7777-777777777771', '2026-06-18', 'Board game turn taking', 'Planned', now()),
    ('88888888-8888-8888-8888-888888888882', '77777777-7777-7777-7777-777777777771', '2026-06-20', 'Draw a picture about our walk', 'Planned', now())
on conflict (id) do nothing;

insert into planned_learning_session_goals (planned_learning_session_id, goal_id)
values
    ('88888888-8888-8888-8888-888888888881', '66666666-6666-6666-6666-666666666661'),
    ('88888888-8888-8888-8888-888888888882', '66666666-6666-6666-6666-666666666662')
on conflict (planned_learning_session_id, goal_id) do nothing;

insert into planned_learning_session_readiness_outcomes (planned_learning_session_id, readiness_outcome_id)
values
    ('88888888-8888-8888-8888-888888888881', '55555555-5555-5555-5555-555555555552'),
    ('88888888-8888-8888-8888-888888888882', '55555555-5555-5555-5555-555555555553')
on conflict (planned_learning_session_id, readiness_outcome_id) do nothing;

insert into daily_logs (id, household_id, child_id, log_date, created_on)
values
    ('99999999-9999-9999-9999-999999999991', '11111111-1111-1111-1111-111111111111', '44444444-4444-4444-4444-444444444441', '2026-06-17', now())
on conflict (id) do nothing;

insert into completed_activities (id, daily_log_id, title, created_on)
values
    ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1', '99999999-9999-9999-9999-999999999991', 'Played a matching game and waited for turns', now())
on conflict (id) do nothing;

insert into completed_activity_readiness_outcomes (completed_activity_id, readiness_outcome_id)
values
    ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1', '55555555-5555-5555-5555-555555555552')
on conflict (completed_activity_id, readiness_outcome_id) do nothing;

insert into reading_entries (id, daily_log_id, title, author, child_response, created_on)
values
    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1', '99999999-9999-9999-9999-999999999991', 'Brown Bear, Brown Bear, What Do You See?', 'Bill Martin Jr.', 'Named animals and repeated some colour words.', now())
on conflict (id) do nothing;

insert into routine_entries (id, daily_log_id, routine_name, notes, created_on)
values
    ('cccccccc-cccc-cccc-cccc-ccccccccccc1', '99999999-9999-9999-9999-999999999991', 'Morning routine', 'Put shoes near the door and followed two simple steps with one reminder.', now())
on conflict (id) do nothing;

commit;
